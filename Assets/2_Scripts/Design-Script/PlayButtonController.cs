using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayButtonController : MonoBehaviourPunCallbacks
{
    public Button playButton;
    public Text statusText;
    PhotonCallback photonCallback;
    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        playButton.interactable = true;
        PhotonNetwork.AddCallbackTarget(this);
        photonCallback = FindObjectOfType<PhotonCallback>();
        photonCallback.playButtonController = this;
        playButton.onClick.AddListener(QuickMatchCall);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void QuickMatchCall() 
    {
        photonCallback.QuickMatch();
    }
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
