////////////////////////////////////////////////////
/// ConnectionManager.cs
/// 
/// This script is responsible for creating a WebSocket connection to the Hive blockchain.

using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;//Instance of the ConnectionManager class

    private ClientWebSocket socket;//WebSocket connection

    public ConnectionManager() { }//Constructor

    public static ConnectionManager Instance//Instance property
    {
        get//Get the instance
        {
            if (instance == null)
            {
                instance = new ConnectionManager();//Create a new instance of the ConnectionManager class
            }
            return instance;//Return the instance
        }
    }

    private async void Awake()
    {
        if (instance == null)//If the instance is null
        {
            instance = this;//Set the instance to this
        }
        else
        {
            Destroy(gameObject);//Destroy the game object
        }
        if (socket == null)//If the socket is null
        {
            socket = await Connect("wss://hive-auth.arcange.eu");//Connect to the Hive blockchain
            Debug.Log("WebSocket connection established.");//Log the connection status
        }
    }

    public async Task<ClientWebSocket> Connect(string host)//Connect to the Hive blockchain
    {
        ClientWebSocket socket = new ClientWebSocket();//Create a new WebSocket connection
        Uri serverUri = new Uri(host);//Create a new URI
        await socket.ConnectAsync(serverUri, CancellationToken.None);//Connect to the server
        return socket;//Return the socket
    }


    public ClientWebSocket GetSocket()//Get the WebSocket connection
    {
        return socket;//Return the socket
    }

    public async Task Disconnect()//Disconnect from the server
    {
        if (socket != null)//If the socket is not null
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);//Close the connection
            socket = null;//Set the socket to null
        }
    }

}
