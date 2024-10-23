////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PlayButtonController.cs
//FileType: C# Source file
//Description : This script is used to handle PVP mode match making
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine.UI;
using Photon.Pun;

public class PlayButtonController : MonoBehaviourPunCallbacks
{
    public Button playButton;               //Play button reference
    public Text statusText;                 //Connection status text
    PhotonCallback photonCallback;          //Photon callback reference

    /// <summary>
    /// Add event listner  to play button and setup callback 
    /// </summary>
    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        playButton.interactable = true;
        PhotonNetwork.AddCallbackTarget(this);
        photonCallback = FindObjectOfType<PhotonCallback>();
        photonCallback.playButtonController = this;
        playButton.onClick.AddListener(QuickMatchCall);
    }
    /// <summary>
    /// Remove callbacks
    /// </summary>
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    /// <summary>
    /// Start Quick match
    /// </summary>
    public void QuickMatchCall() 
    {
        photonCallback.QuickMatch();
    }
    /// <summary>
    /// Join random room
    /// </summary>
    private void OnPlayButtonClick()
    {
        statusText.text = "Finding match...";
        PhotonCallback photonCallback = FindObjectOfType<PhotonCallback>();
        if (photonCallback != null)
        {
            photonCallback.JoinRandomRoom();
        }
    }
}
