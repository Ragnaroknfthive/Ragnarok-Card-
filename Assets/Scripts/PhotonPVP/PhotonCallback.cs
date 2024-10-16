////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PhotonCallback.cs
//FileType: C# Source file
//Description : This is a c# script used to handle photon network callbacks
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class PhotonCallback : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private byte maxPlayers = 2;                                                    //Max number of players allowed in room
static string AppID = "b155e53a-8156-43d7-ab29-461afb3885bb";                       //Photon App Id 

    public bool ConnectToCustomRegion = false;                                      //Used to check reguib selection mode- auto or menual
    public bool ConnetToMenualRegion = false;                                       //True in case user selects cloud region manually
    public bool autoReconnect;                                                      //If true automatically reconnects to photon network
    public PlayButtonController playButtonController;
    public DisplayOpponentProfile displayOpponentProfile;                           //Opponent profile display script reference
    private Coroutine network_routine;                                              //coroutine reference
    public bool isReadyToFindMatch = false;                                         //True if player is ready for match making
    /// <summary>
    /// Trigger Network connection logic
    /// </summary>
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Trigger coroutine for auto reconnect
    /// </summary>
    void Start()
    {
        autoReconnect = true;
        if (network_routine != null)
            network_routine = StartCoroutine(Reconnection());

    }
	/// <summary>
    /// Connect to selected cloud region in case player is already connected to a region
    /// </summary>
    void Update()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady)
        {
            if (ConnectToCustomRegion)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.ConnectToRegion(PlayerPrefs.HasKey("pun_region") ? MenuUI.Get().getSelectedRegion() : "us");
                ConnectToCustomRegion = false;
            }
        }

    }
    /// <summary>
    /// Automatically connect the client to the server every 2 seconds if not connected:
    /// </summary>
    /// <returns></returns>
    IEnumerator Reconnection()
    {
        while (autoReconnect)
        {
            yield return new WaitForSeconds(2f);

            Debug.LogError(PhotonNetwork.IsConnected);
            if (!PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState == ClientState.ConnectingToMasterServer)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.ConnectToRegion(MenuUI.Get().getSelectedRegion());
            }
        }
    }
    /// <summary>
    ///  This callback is triggered when user is connected to master server
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master " + PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
       // Debug.Log(RegionController.GetIndexOfRegion(PhotonNetwork.CloudRegion));
      RegionController.Get().SetRegion( RegionController.GetIndexOfRegion(PhotonNetwork.CloudRegion));
        isReadyToFindMatch = true;
    }
    /// <summary>
    ///  This callback is triggered when user joins lobby on photon network
    /// </summary>
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        if (GameObject.FindObjectOfType<MenuUI>())
        {
            MenuUI UI = FindObjectOfType<MenuUI>();
            UI.UpdatePlayButtonText();
        }
        isReadyToFindMatch = true;
        if(string.IsNullOrEmpty(DisplayAccount.HiveProfileName)==false) 
        {
            PhotonNetwork.LocalPlayer.NickName = DisplayAccount.HiveProfileName+"_"+Random.Range(99,199);
        }
        else if(PlayerPrefs.HasKey("HiveProfileName"))
        {
            DisplayAccount.HiveProfileName = PlayerPrefs.GetString("HiveProfileName");
            PhotonNetwork.LocalPlayer.NickName = DisplayAccount.HiveProfileName + "_" + Random.Range(99,199);
        }
        else 
        {
            PhotonNetwork.LocalPlayer.NickName = "Player_" + Random.Range(99,199);
        }
        ExitGames.Client.Photon.Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        if(!playerProperties.ContainsKey(GameData.hasProfileConst))
        { playerProperties.Add(GameData.hasProfileConst,GameData.hasProfileImage ? 1 : 0); }
        else
        {
            playerProperties[GameData.hasProfileConst] = GameData.hasProfileImage ? 1 : 0;
           
        }

        if(!playerProperties.ContainsKey(GameData.profileUrl))
        { playerProperties.Add(GameData.profileUrl,GameData.playerProfileUrl); }
        else
        {
            playerProperties[GameData.profileUrl]=GameData.playerProfileUrl;
            Debug.LogError(playerProperties[GameData.profileUrl]);
        }

        if(!playerProperties.ContainsKey(GameData.dummyProfileImageIndex))
        { playerProperties.Add(GameData.dummyProfileImageIndex,GameData.hasProfileImage ? -1 : GameData.dummyProfileIndex); }
        else
        {
            playerProperties[GameData.dummyProfileImageIndex] = GameData.dummyProfileIndex;
           
        }
        //PhotonNetwork.LocalPlayer.SetCustomProperties=playerProperties;
        Debug.Log("Joined Lobby"+ DisplayAccount.HiveProfileName);
    }
    /// <summary>
    /// Join random room available for playing
    /// </summary>
    public void QuickMatch()
    {
        if(DeckManager.instance.playerDeck.Count < 33)
        {
            DeckManager.instance.AddAll();
            // MenuUI.Get().ShowMsg("Please complete your deck...",true,true);
            // return;
        }
        if(PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady)
        {

            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //ConnectToCustomRegion = true;
            PhotonNetwork.ConnectUsingSettings();
            //PhotonNetwork.ConnectToRegion(PlayerPrefs.HasKey("pun_region") ? MenuUI.Get().getSelectedRegion() : "us");
        }

    }
    /// <summary>
    /// Join any random room available or create new
    /// </summary>
    public void JoinRandomRoom()
    {
        if (isReadyToFindMatch)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.LogWarning("Client is not ready for matchmaking.");
        }
    }

    /// <summary>
    /// Update room status text 
    /// </summary>
    private void StartGame()
    {
        playButtonController.statusText.text = "Room created";

        string opponentAccount = GetOpponentAccount();
        if (!string.IsNullOrEmpty(opponentAccount))
        {
            displayOpponentProfile.DisplayOpponent(opponentAccount);
        }
        PhotonNetwork.LoadLevel("Game");
    }
    /// <summary>
    /// Hive : Get opponent player's hive account information
    /// </summary>
    /// <returns></returns>
    private string GetOpponentAccount()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                if (player.CustomProperties.TryGetValue("hive_account", out object account))
                {
                    string accountInfo = account.ToString();
                   // Debug.Log("Player" + accountInfo);
                    return accountInfo;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// Create new room
    /// </summary>
    private void CreateRoom()
    {
        if(DeckManager.instance.playerDeck.Count < 33)
        {
           // MenuUI.Get().ShowMsg("Please complete your deck...",true,true);
            //return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.CleanupCacheOnLeave = false;
        PhotonNetwork.CreateRoom(null,roomOptions,null);
        MenuUI.Get().ShowMsg("Room created waiting for players...",true);
    }
    /// <summary>
    ///  This callback is triggered when user failed to join random room
    /// </summary>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
      //  RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers,CleanupCacheOnLeave = false };
      //  PhotonNetwork.CreateRoom(null,roomOptions,null);
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady)
        {
            CreateRoom();

            if(FindObjectOfType<PlayButtonController>() != null)
            {
                FindObjectOfType<PlayButtonController>().statusText.text = "New Room Created";
            }
        }
        else
        {
            if(FindObjectOfType<PlayButtonController>() != null)
            {
                FindObjectOfType<PlayButtonController>().statusText.text = "Connection Lost..";
            }
            PhotonNetwork.ConnectUsingSettings();

          
            //PhotonNetwork.ConnectToRegion(PlayerPrefs.HasKey("pun_region") ? MenuUI.Get().getSelectedRegion() : "us");
        }

    }
    /// <summary>
    ///  This callback is triggered when user joins random room
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.PlayerList.Length == maxPlayers)
        {
            //PhotonNetwork.PlayerList[0].NickName =  "Player 1";
            //PhotonNetwork.PlayerList[1].NickName = "Player 2";
            playButtonController.statusText.text = "Room created";

            string opponentAccount = GetOpponentAccount();
            if(!string.IsNullOrEmpty(opponentAccount))
            {
                displayOpponentProfile.DisplayOpponent(opponentAccount);
            }
            DeckManager.instance.SetOpponentDeck();
            PhotonNetwork.LoadLevel("Game");
            
        }
        else
        {
            MenuUI.Get().ShowMsg("Waiting for other players..", true);
        }
    }
    /// <summary>
    /// Leave photon room
    /// </summary>
    public void CancelMatch()
    {
        PhotonNetwork.LeaveRoom();
        MenuUI.Get().ErrorDisp.SetActive(false);
    }
    /// <summary>
    ///  This callback is triggered to players devices which are already in room and new user joins a room 
    /// </summary>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == maxPlayers)
        {
            playButtonController.statusText.text = "Room created";

            string opponentAccount = GetOpponentAccount();
            if(!string.IsNullOrEmpty(opponentAccount))
            {
                displayOpponentProfile.DisplayOpponent(opponentAccount);
            }
           // PhotonNetwork.PlayerList[0].NickName = "Player 1";
           // PhotonNetwork.PlayerList[1].NickName = "Player 2";
            DeckManager.instance.SetDeck();
            PhotonNetwork.LoadLevel("Game");

        }
        else
        {
            MenuUI.Get().ShowMsg("Waiting for other players..", true);
        }
    }





}
