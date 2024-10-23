////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: StartButtonController.cs
//FileType: C# Source file
//Description : This is a c# script  for bu
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StartButtonController : MonoBehaviour
{
    public Button startButton;
    public RegionController regionController;
    public string nextSceneName = "NextScene";

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    private void OnStartButtonClick()
    {
        string selectedRegion = regionController.getSelectedRegion();
        StartCoroutine(RegionController.ConnnectToNewRegion(selectedRegion));
        PhotonNetwork.LoadLevel(nextSceneName);
    }
}
