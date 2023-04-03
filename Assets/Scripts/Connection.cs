using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    private ClientWebSocket socket;

    public ConnectionManager() { }

    // string hostName = connectManager.GetHostName();
    public static ConnectionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConnectionManager();
            }
            return instance;
        }
    }

    private async void Awake()
    {
        // Make sure there is only one instance of ConnectionManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Create a WebSocket connection if it doesn't exist yet
        if (socket == null)
        {
            // DontDestroyOnLoad(this.gameObject);
            socket = await Connect("wss://hive-auth.arcange.eu");
            Debug.Log("WebSocket connection established.");
        }
    }

    public async Task<ClientWebSocket> Connect(string host)
    {
        ClientWebSocket socket = new ClientWebSocket();
        Uri serverUri = new Uri(host);
        await socket.ConnectAsync(serverUri, CancellationToken.None);
        return socket;
    }


    public ClientWebSocket GetSocket()
    {
        return socket;
    }

    public async Task Disconnect()
    {
        if (socket != null)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
            socket = null;
        }
    }

}
