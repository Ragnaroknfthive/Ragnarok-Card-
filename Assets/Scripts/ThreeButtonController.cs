using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThreeButtonController : MonoBehaviour
{
    public Button sceneButton;
    public Button openPanelButton;
    public Button registrationButton;
    public Button closePanelButton;
    public GameObject panel;

    void Start()
    {
        sceneButton.onClick.AddListener(ChangeScene);
        openPanelButton.onClick.AddListener(OpenPanel);
        registrationButton.onClick.AddListener(OpenPanel);
        closePanelButton.onClick.AddListener(ClosePanel);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("HiveLogin");
    }

    void OpenPanel()
    {
        panel.SetActive(true);
    }

    void ClosePanel()
    {
        panel.SetActive(false);
    }
}
