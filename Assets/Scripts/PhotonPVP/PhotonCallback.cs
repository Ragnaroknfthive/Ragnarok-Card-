using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Debug = UnityEngine.Debug;

public class PhotonCallback : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private byte maxPlayers = 2;
    public PlayButtonController playButtonController;
    public DisplayOpponentProfile displayOpponentProfile;

    public bool isReadyToFindMatch = false;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master " + PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
        isReadyToFindMatch = true;
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        isReadyToFindMatch = true;
    }

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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers, CleanupCacheOnLeave = false };
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called");

        if (PhotonNetwork.PlayerList.Length == maxPlayers)
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom() called");

        if (PhotonNetwork.PlayerList.Length == maxPlayers)
        {
            StartGame();
        }
    }

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
                    Debug.Log("Player" + accountInfo);
                    return accountInfo;
                }
            }
        }
        return null;
    }
}
