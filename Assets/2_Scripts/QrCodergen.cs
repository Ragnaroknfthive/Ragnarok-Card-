////////////////////////////////////////////////////////////////
///QrCodergen.cs
///
///This script generates a QR code for authentication and connects to a WebSocket server.
///It includes methods to send and receive messages from the server, process messages,
///and load the next scene after authentication.
///The script is attached to a GameObject with the UI elements to be controlled.

using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace generator
{
    public class QrCodergen : MonoBehaviour
    {
        private string m_auth_key; // Authentication key
        public TMPro.TMP_InputField txtUsername; // Input field for username
        public RawImage picQRCode; // Raw image to display QR code
        public Button connectButton; // Button to initiate connection
        public Text errorMessage; // Text to display error messages
        public Text tokenText; // Text to display authentication token
        public RawImage successImage; // Raw image to indicate success
        public GameObject qrCodePanel; // Panel containing the QR code
        string m_auth_host = "wss://hive-auth.arcange.eu"; // Authentication host URL
        private ClientWebSocket socket; // WebSocket client

        public string AuthToken { get; private set; } // Property to store authentication token

        private readonly SemaphoreSlim sendReceiveSemaphore = new SemaphoreSlim(1); // Semaphore for thread safety
        private async void Start()
        {
            ClientWebSocket socket = await ConnectionManager.Instance.Connect(m_auth_host); // Connect to the authentication server
            connectButton.onClick.AddListener(OnConnectButtonClick); // Add event listener for button click
        }

        void ProcessMessage(string msg)
        {
            successImage.gameObject.SetActive(false); // Hide success image
            Debug.Log(msg); // Log received message
            JObject JMsg = JObject.Parse(msg); // Parse the message as JSON

            switch ((string)JMsg["cmd"]) // Check the command in the message
            {
                case "auth_wait":
                    try
                    {
                        string json =
                            new JObject(
                                new JProperty("account", txtUsername.text), // User account
                                new JProperty("uuid", JMsg["uuid"]), // Unique identifier
                                new JProperty("key", m_auth_key), // Authentication key
                                new JProperty("host", m_auth_host) // Host URL
                                ).ToString();
                        Debug.Log("JSON payload: " + json); // Log JSON payload

                        string URI = "has://auth_req/" + Convert.ToBase64String(Encoding.UTF8.GetBytes(json)); // Create URI for QR code
                        Debug.Log("QR code URI: " + URI); // Log QR code URI
                        var writer = new BarcodeWriter
                        {
                            Format = BarcodeFormat.QR_CODE, // Specify barcode format
                            Options = new EncodingOptions
                            {
                                Height = 256, // Set height of QR code
                                Width = 256, // Set width of QR code
                                Margin = 1 // Set margin for QR code
                            }
                        };

                        var qrCodeTexture = new Texture2D(256, 256); // Create texture for QR code
                        var encoded = new Texture2D(256, 256, TextureFormat.RGB24, false, true); // Create encoded texture
                        var color32 = writer.Write(URI); // Write the QR code
                        qrCodeTexture.SetPixels32(color32); // Set pixels for the QR code texture

                        qrCodeTexture.Apply(); // Apply texture changes

                        if (picQRCode != null)
                        {
                            picQRCode.texture = qrCodeTexture; // Assign texture to the RawImage
                        }
                        successImage.gameObject.SetActive(false); // Hide success image
                        tokenText.text = ""; // Clear token text

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Failed to process auth_wait message: " + ex.ToString()); // Log error
                        errorMessage.text = "Failed to process auth_wait message: " + ex.Message; // Display error message
                    }
                    break;
                case "auth_ack":
                    try
                    {
                        picQRCode.gameObject.SetActive(false); // Hide QR code image
                        string decrypted = CryptoJS.Decrypt((string)JMsg["data"], m_auth_key); // Decrypt data
                        JObject JData = JObject.Parse(decrypted); // Parse decrypted data
                        string token = (string)JData["token"]; // Get authentication token
                        ulong expire = (ulong)JData["expire"]; // Get token expiration
                        Debug.Log(string.Format("Authenticated with token: {0}", token)); // Log authentication token
                        tokenText.text = string.Format("Authenticated with token: {0}", token); // Display authentication token
                        successImage.gameObject.SetActive(true); // Show success image

                        AuthToken = token; // Store authentication token
                        PlayerPrefs.SetString("AuthToken", token); // Save token to PlayerPrefs
                        string account = txtUsername.text; // Get username
                        PlayerPrefs.SetString("account", account); // Save account to PlayerPrefs
                        PlayerPrefs.SetString("m_auth_key", m_auth_key); // Save authentication key to PlayerPrefs
                        PlayerPrefs.Save(); // Save PlayerPrefs changes

                        StartCoroutine("LoadNextScene"); // Load the next scene
                    }
                    catch (Exception ex)
                    {
                        picQRCode.gameObject.SetActive(false); // Hide QR code image
                        Debug.Log("Decryption failed: " + ex.Message); // Log decryption error
                    }
                    break;
                case "auth_err":
                    picQRCode.gameObject.SetActive(false); // Hide QR code image
                    string error = (string)JMsg["error"]; // Get error message
                    Debug.Log("Authentication error: " + error); // Log authentication error
                    errorMessage.text = error; // Display error message
                    break;
                case "auth_status":
                    try
                    {
                        string status = (string)JMsg["status"]; // Get authentication status
                        Debug.Log("Authentication status: " + status); // Log authentication status
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Error processing authentication status message: " + ex.Message); // Log error
                    }
                    break;
                default:
                    Debug.Log("Unknown command: " + (string)JMsg["cmd"]); // Log unknown command
                    break;
            }
        }


        // Sends data to the WebSocket server asynchronously.
        static async Task Send(ClientWebSocket socket, string data) =>
            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, true, CancellationToken.None);

        // Receives messages from the WebSocket server asynchronously.
        async Task Receive(ClientWebSocket socket)
        {
            // Create a buffer for incoming messages.
            var buffer = new ArraySegment<byte>(new byte[2048]);

            // Continuously receive messages from the server.
            do
            {
                WebSocketReceiveResult result;

                // Use a MemoryStream to assemble the received message.
                using (MemoryStream ms = new MemoryStream())
                {
                    // Continue receiving until the end of the message is reached.
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count); // Write received bytes to the memory stream.
                    } while (!result.EndOfMessage); // Keep receiving until the entire message is received.

                    // If the message is a close command, break the loop.
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin); // Reset the position of the memory stream.
                    using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        // Process the complete message once received.
                        ProcessMessage(await reader.ReadToEndAsync());
                    }
                }
            } while (true); // Continue receiving indefinitely until a close message is received.
        }

        // Loads the next scene after a delay.
        async Task<IEnumerator> LoadNextScene()
        {
            await Task.Delay(5000); // Wait for 5 seconds before loading the next scene.
            SceneManager.LoadScene("LoadingScreen"); // Load the loading screen scene.

            return null; // Return null as no additional yield instructions are needed.
        }

        // Handles the connect button click event.
        async void OnConnectButtonClick()
        {
            connectButton.interactable = false; // Disable the button to prevent multiple clicks.
            errorMessage.text = ""; // Clear any previous error messages.

            // Check if the username input is empty and display an error message if so.
            if (string.IsNullOrEmpty(txtUsername.text))
            {
                errorMessage.text = "Please enter a username.";
                return;
            }

            qrCodePanel.SetActive(true); // Activate the QR code panel.
            ClientWebSocket socket = new ClientWebSocket(); // Create a new WebSocket client.

            try
            {
                m_auth_key = Guid.NewGuid().ToString(); // Generate a new authentication key.
                await socket.ConnectAsync(new Uri(m_auth_host), CancellationToken.None); // Connect to the WebSocket server.

                // Prepare the authentication request data.
                string auth_req_data =
                    new JObject(
                        new JProperty("app",
                            new JObject(
                                new JProperty("name", "has-demo-dotnet"),
                                new JProperty("description", "Demo - HiveAuth with .NET")
                            )
                        )
                    ).ToString();

                // Encrypt the authentication request data using the authentication key.
                auth_req_data = CryptoJS.Encrypt(auth_req_data, m_auth_key);

                // Create the payload for the authentication request.
                string payload =
                    new JObject(
                        new JProperty("cmd", "auth_req"),
                        new JProperty("account", txtUsername.text),
                        new JProperty("data", auth_req_data)
                    ).ToString();

                // Send the authentication request payload to the server.
                await Send(socket, payload);

                // Receive the server's response.
                await Receive(socket);
                Debug.Log("Receive completed"); // Log that receiving has completed.
            }
            catch (Exception ex)
            {
                // Display any errors that occur during the connection process.
                errorMessage.text = "Error: " + ex.Message;
                Debug.LogException(ex); // Log the exception for debugging.
            }
        }
    }
}