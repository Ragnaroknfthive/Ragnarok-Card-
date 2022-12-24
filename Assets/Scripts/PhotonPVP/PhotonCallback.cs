using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class PhotonCallback : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private byte maxPlayers = 2;

    // Start is called before the first frame update
    void Start()
    {
        Time.maximumDeltaTime = 0.03f;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master");		
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
        if(GameObject.FindObjectOfType<MenuUI>()) 
        {
            MenuUI UI = FindObjectOfType<MenuUI>();
            UI.UpdatePlayButtonText();
        }
        
	}

    

    private void CreateRoom()
    {
        if(DeckManager.instance.playerDeck.Count < 33){
            MenuUI.Get().ShowMsg("Please complete your deck...",true,true);
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.CleanupCacheOnLeave = false;
        PhotonNetwork.CreateRoom(null, roomOptions, null);
        MenuUI.Get().ShowMsg("Room created waiting for players...");
    }

    public void QuickMatch()
    {
        if(DeckManager.instance.playerDeck.Count < 33){
            DeckManager.instance.AddAll();
            // MenuUI.Get().ShowMsg("Please complete your deck...",true,true);
            // return;
        }
        if(PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady){
            PhotonNetwork.JoinRandomRoom();
        }else{
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if(PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady){
            CreateRoom();
        }else{
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.PlayerList.Length == maxPlayers){
            PhotonNetwork.PlayerList[0].NickName = "Player 1";
            PhotonNetwork.PlayerList[1].NickName = "Player 2";
            DeckManager.instance.SetDeck();
            PhotonNetwork.LoadLevel("Game");
        }else{
            MenuUI.Get().ShowMsg("Waiting for other players..");
        }
    } 

    public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if(PhotonNetwork.PlayerList.Length == maxPlayers){
            PhotonNetwork.PlayerList[0].NickName = "Player 1";
            PhotonNetwork.PlayerList[1].NickName = "Player 2";
            DeckManager.instance.SetDeck();
            PhotonNetwork.LoadLevel("Game");

        }else{
            MenuUI.Get().ShowMsg("Waiting for other players..");
        }
	} 

    
}
