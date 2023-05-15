using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayButtonController : MonoBehaviourPunCallbacks
{
    public Button playButton;
    public Text statusText;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        playButton.interactable = true;
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
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
