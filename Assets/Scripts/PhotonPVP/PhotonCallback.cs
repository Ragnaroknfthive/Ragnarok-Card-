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
    private byte maxPlayers = 2; 
static string AppID = "b155e53a-8156-43d7-ab29-461afb3885bb";

    public bool ConnectToCustomRegion = false;
    public bool ConnetToMenualRegion = false;
    public bool autoReconnect;
    public PlayButtonController playButtonController;
    public DisplayOpponentProfile displayOpponentProfile;
    private Coroutine network_routine;
    public bool isReadyToFindMatch = false;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
	 // Start is called before the first frame update
    void Start()
    {

        
        autoReconnect = true;
        if (network_routine != null)
            network_routine = StartCoroutine(Reconnection());

    }
	  // Update is called once per frame
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

    // This will automatically connect the client to the server every 2 seconds if not connected:
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

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master " + PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
       // Debug.Log(RegionController.GetIndexOfRegion(PhotonNetwork.CloudRegion));
      RegionController.Get().SetRegion( RegionController.GetIndexOfRegion(PhotonNetwork.CloudRegion));
        isReadyToFindMatch = true;
    }

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
    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected to Master " + PhotonNetwork.CloudRegion);
    //    PhotonNetwork.JoinLobby();
    //    isReadyToFindMatch = true;
    //}

    //public override void OnJoinedLobby()
    //{
    //    base.OnJoinedLobby();
    //    isReadyToFindMatch = true;
    //    Debug.Log("Joined Lobby");
    //}

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

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers, CleanupCacheOnLeave = false };
    //    PhotonNetwork.CreateRoom(null, roomOptions, null);
    //}

    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("OnJoinedRoom() called");

    //    if (PhotonNetwork.PlayerList.Length == maxPlayers)
    //    {
    //        StartGame();
    //    }
    //}

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    Debug.Log("OnPlayerEnteredRoom() called");

    //    if (PhotonNetwork.PlayerList.Length == maxPlayers)
    //    {
    //        StartGame();
    //    }
    //}

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

    public void CancelMatch()
    {
        PhotonNetwork.LeaveRoom();
        MenuUI.Get().ErrorDisp.SetActive(false);
    }

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
