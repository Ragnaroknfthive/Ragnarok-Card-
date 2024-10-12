using System.Collections; // Namespace for collections and enumerators
using System.Collections.Generic; // Namespace for generic collections
using UnityEngine; // Core Unity namespace
using UnityEngine.SceneManagement; // Namespace for scene management
using UnityEngine.UI; // Namespace for UI elements

/// <summary>
/// Manages UI interactions for a three-button interface in Unity.
/// </summary>
public class ThreeButtonController : MonoBehaviour
{
    // UI Button references
    public Button sceneButton; // Button to change the scene
    public Button openPanelButton; // Button to open a UI panel
    public Button registrationButton; // Button to open a UI panel (same functionality as openPanelButton)
    public Button closePanelButton; // Button to close the UI panel
    public GameObject panel; // The UI panel to be shown or hidden

    /// <summary>
    /// Initializes the button listeners when the script starts.
    /// </summary>
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

    /// <summary>
    /// Changes the current scene to the "HiveLogin" scene.
    /// </summary>
    void ChangeScene()
    {
        SceneManager.LoadScene("HiveLogin");
    }

    /// <summary>
    /// Opens the specified panel by setting it to active.
    /// </summary>
    void OpenPanel()
    {
        panel.SetActive(true);
    }

    /// <summary>
    /// Closes the specified panel by setting it to inactive.
    /// </summary>
    void ClosePanel()
    {
        panel.SetActive(false);
    }
}

