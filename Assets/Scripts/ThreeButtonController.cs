////////////////////////////////////////////////////////////////////////////////
///ThreeButtonController.cs
///
///This script manages UI interactions for a three-button interface in Unity.
///It includes methods to change the scene, open a panel, and close a panel.
///The script is attached to a GameObject with the UI elements to be controlled.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ThreeButtonController : MonoBehaviour
{
    // UI Button references
    public Button sceneButton; // Button to change the scene
    public Button openPanelButton; // Button to open a UI panel
    public Button registrationButton; // Button to open a UI panel (same functionality as openPanelButton)
    public Button closePanelButton; // Button to close the UI panel
    public GameObject panel; // The UI panel to be shown or hidden

    void Start()
    {
        // Add listener to change scene when the sceneButton is clicked
        sceneButton.onClick.AddListener(ChangeScene);

        // Add listener to open the panel when the openPanelButton is clicked
        openPanelButton.onClick.AddListener(OpenPanel);

        // Add listener to open the panel when the registrationButton is clicked
        registrationButton.onClick.AddListener(OpenPanel);

        // Add listener to close the panel when the closePanelButton is clicked
        closePanelButton.onClick.AddListener(ClosePanel);
    }
    /// Changes the current scene to the "HiveLogin" scene.
    void ChangeScene()
    {
        SceneManager.LoadScene("HiveLogin");
    }

    /// Opens the specified panel by setting it to active.
    void OpenPanel()
    {
        panel.SetActive(true);
    }

    /// Closes the specified panel by setting it to inactive.
    void ClosePanel()
    {
        panel.SetActive(false);// Set the panel to inactive
    }
}

