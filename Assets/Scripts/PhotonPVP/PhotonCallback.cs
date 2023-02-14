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
    static string AppID = "b155e53a-8156-43d7-ab29-461afb3885bb";

    public bool ConnectToCustomRegion = false;
    public bool ConnetToMenualRegion = false;
    public bool autoReconnect;

    public Coroutine network_routine;
    private void Awake()
    {
        Time.maximumDeltaTime = 0.03f;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Start is called before the first frame update
    void Start()
    {

        //PhotonNetwork.ConnectToRegion(PlayerPrefs.HasKey("pun_region") ? MenuUI.Get().getSelectedRegion() : "us");
        // try
        // {

        // }
        // catch
        // {
        //     // PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = AppID;
        //     // PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = PlayerPrefs.HasKey("pun_region") ? MenuUI.Get().getSelectedRegion() : "";
        //     PhotonNetwork.ConnectUsingSettings();
        // }
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

    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        if (GameObject.FindObjectOfType<MenuUI>())
        {
            MenuUI UI = FindObjectOfType<MenuUI>();
            UI.UpdatePlayButtonText();
        }

    }





    private void CreateRoom()
    {
        if (DeckManager.instance.playerDeck.Count < 33)
        {
            MenuUI.Get().ShowMsg("Please complete your deck...", true, true);
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.CleanupCacheOnLeave = false;
        PhotonNetwork.CreateRoom(null, roomOptions, null);
        MenuUI.Get().ShowMsg("Room created waiting for players...", true);
    }

    public void QuickMatch()
    {
        if (DeckManager.instance.playerDeck.Count < 33)
        {
            DeckManager.instance.AddAll();
            // MenuUI.Get().ShowMsg("Please complete your deck...",true,true);
            // return;
        }
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady)
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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady)
        {
            CreateRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            //PhotonNetwork.ConnectToRegion(PlayerPrefs.HasKey("pun_region") ? MenuUI.Get().getSelectedRegion() : "us");
        }

    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.PlayerList.Length == maxPlayers)
        {
            PhotonNetwork.PlayerList[0].NickName = "Player 1";
            PhotonNetwork.PlayerList[1].NickName = "Player 2";
            DeckManager.instance.SetDeck();
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
            PhotonNetwork.PlayerList[0].NickName = "Player 1";
            PhotonNetwork.PlayerList[1].NickName = "Player 2";
            DeckManager.instance.SetDeck();
            PhotonNetwork.LoadLevel("Game");

        }
        else
        {
            MenuUI.Get().ShowMsg("Waiting for other players..", true);
        }
    }




}
