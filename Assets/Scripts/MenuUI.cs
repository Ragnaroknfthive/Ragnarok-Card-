using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;

/// <summary>
/// This class manages the user interface for the main menu,
/// including region selection, connecting to Photon servers, and navigating between different menu options.
/// </summary>
public class MenuUI : MonoBehaviour
{
    // Singleton instance of the MenuUI
    private static MenuUI mui;

    // GameObject to display error messages
    public GameObject ErrorDisp;

    // Reference to PhotonView for network communication
    private PhotonView photonView;

    // UI components for displaying messages and buttons
    public Text MsgTxt;
    public Button PlayBtn;

    // UI components for the logo and splash screen
    public GameObject Logo, Splash;
    public Dropdown dropdown;
    public Text regionTXT;

    // List of available regions for connecting to Photon
    public List<string> regions = new List<string>() {
        "Asia", "Australia", "Canada, East", "Europe",
        "India", "Japan", "Russia", "South America",
        "South Korea", "USA, East", "USA, West",
        "Russia, East", "South Africa", "Turkey"
    };

    // Application ID for Photon
    static string AppID = "b155e53a-8156-43d7-ab29-461afb3885bb";

    // List of menu options available in the UI
    public List<MenuOption> menuOptions = new List<MenuOption>();

    // Sprites for UI elements
    public Sprite deckCardHoverSprite, deckHoverSprite;

    /// <summary>
    /// Initializes the MenuUI instance and sets up the initial state of the UI.
    /// </summary>
    private void Awake()
    {
        mui = this;
        if (regionTXT)
            regionTXT.text = "Connecting to...";
        if (PlayBtn)
            PlayBtn.interactable = false;
        if (dropdown)
            dropdown.interactable = false;
        InitOptions();
    }

    /// <summary>
    /// Initializes the dropdown options with the available regions.
    /// </summary>
    public void InitOptions()
    {
        if (!dropdown) return;

        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        foreach (var item in regions)
        {
            optionDatas.Add(new Dropdown.OptionData(item));
        }
        dropdown.options = optionDatas; // Set the dropdown options
        dropdown.value = PlayerPrefs.GetInt("pun_region", 0); // Set the default selected region from PlayerPrefs
    }

    /// <summary>
    /// Gets the currently selected region code.
    /// </summary>
    /// <returns>Region code as a string.</returns>
    public string getSelectedRegion()
    {
        return GetRegionCode(PlayerPrefs.GetInt("pun_region", 0));
    }

    /// <summary>
    /// Maps the dropdown selection index to the corresponding region code.
    /// </summary>
    /// <param name="s">Index of the selected region.</param>
    /// <returns>Region code as a string.</returns>
    public string GetRegionCode(int s)
    {
        string region = "us"; // Default region is US
        switch (s)
        {
            case 0: region = "asia"; break;
            case 1: region = "au"; break;
            case 2: region = "cae"; break;
            case 3: region = "eu"; break;
            case 4: region = "in"; break;
            case 5: region = "jp"; break;
            case 6: region = "ru"; break;
            case 7: region = "sa"; break;
            case 8: region = "kr"; break;
            case 9: region = "us"; break;
            case 10: region = "usw"; break;
            case 11: region = "rue"; break;
            case 12: region = "za"; break;
            case 13: region = "tr"; break;
        }
        return region;
    }

    /// <summary>
    /// Called when a new region is selected from the dropdown.
    /// Updates the UI and initiates a connection to the selected region.
    /// </summary>
    public void OnSelectRegion()
    {
        string region = GetRegionCode(dropdown.value); // Get region code based on selected dropdown value

        // Update UI elements to indicate connection status
        if (regionTXT)
            regionTXT.text = "Connecting to...";
        if (PlayBtn)
            PlayBtn.interactable = false;
        if (dropdown)
            dropdown.interactable = false;

        // Start the coroutine to connect to the new region
        if (dropdown)
            StartCoroutine(ConnnectToNewRegion(region));
    }

    /// <summary>
    /// Coroutine to handle connecting to a new region.
    /// </summary>
    /// <param name="region">The region to connect to.</param>
    /// <returns>Yield instruction to wait during the connection process.</returns>
    IEnumerator ConnnectToNewRegion(string region)
    {
        PhotonNetwork.Disconnect(); // Disconnect from the current server
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected); // Wait until disconnected

        Debug.Log("Connecting to region: " + region);
        PhotonNetwork.ConnectToRegion(region); // Connect to the new region
        PlayerPrefs.SetInt("pun_region", dropdown.value); // Save selected region to PlayerPrefs
    }

    /// <summary>
    /// Initializes the menu UI on start.
    /// Displays the home screen if menu options are available.
    /// Fades in the logo and splash screen.
    /// </summary>
    void Start()
    {
        if (menuOptions.Count > 0)
        {
            ShowHomeScreen(); // Show the home screen
        }
        if (!Logo) return;

        // Fade in the logo and then fade it out along with the splash screen
        LeanTween.alphaCanvas(Logo.GetComponent<CanvasGroup>(), 1f, 0.7f).setDelay(0.3f).setOnComplete(() =>
        {
            LeanTween.alphaCanvas(Logo.GetComponent<CanvasGroup>(), 0f, 0.3f).setDelay(0.7f);
            LeanTween.alphaCanvas(Splash.GetComponent<CanvasGroup>(), 0f, 0.3f).setDelay(0.7f);
        });
    }

    /// <summary>
    /// Hides the error message display.
    /// Leaves the current Photon room and disables the error display.
    /// </summary>
    public void HideMsg()
    {
        PhotonNetwork.LeaveRoom(); // Leave the room if in one
        ErrorDisp.SetActive(false); // Disable the error display
    }

    /// <summary>
    /// Coroutine to hide the error display after a delay.
    /// </summary>
    /// <returns>Yield instruction to wait before hiding the display.</returns>
    IEnumerator Hide()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        ErrorDisp.gameObject.SetActive(false); // Disable the error display
    }

    /// <summary>
    /// Updates the play button text and enables the play button and dropdown after connecting.
    /// </summary>
    public void UpdatePlayButtonText()
    {
        if (!PlayBtn) return;

        dropdown.interactable = true; // Enable dropdown for region selection
        regionTXT.text = "Connected to"; // Update text to show connection status
        PlayBtn.interactable = true; // Enable the play button
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit()
    {
        Application.Quit(); // Exit the application
    }

    /// <summary>
    /// Displays the home screen by opening the corresponding menu option.
    /// </summary>
    public void ShowHomeScreen()
    {
        OpenMenuOption(MenuOptionType.Home); // Open the home menu option
    }

    /// <summary>
    /// Displays the play screen by opening the corresponding menu option.
    /// </summary>
    public void ShowPlayScreen()
    {
        OpenMenuOption(MenuOptionType.Play); // Open the play menu option
    }

    /// <summary>
    /// Displays the profile screen by opening the corresponding menu option.
    /// </summary>
    public void ShowPRofileScreen()
    {
        OpenMenuOption(MenuOptionType.Profile); // Open the profile menu option
    }

    /// <summary>
    /// Displays the deck screen by opening the corresponding menu option.
    /// </summary>
    public void ShowDeckScreen()
    {
        OpenMenuOption(MenuOptionType.Decks); // Open the decks menu option
    }

    /// <summary>
    /// Displays the shop screen by opening the corresponding menu option.
    /// </summary>
    public void ShowShopScreen()
    {
        OpenMenuOption(MenuOptionType.Shop); // Open the shop menu option
    }

    /// <summary>
    /// Opens the specified menu option and hides all others.
    /// </summary>
    /// <param name="optionType">The type of menu option to open.</param>
    public void OpenMenuOption(MenuOptionType optionType)
    {
        // Iterate through all menu options
        foreach (MenuOption item in menuOptions)
        {
            // Activate the selected menu option's screen, deactivate others
            item.optionScreen.SetActive(item.optionType == optionType);
        }
    }
}

/// <summary>
/// Enum representing different menu option types.
/// </summary>
[System.Serializable]
public enum MenuOptionType { Home, Play, Profile, Decks, Shop }

/// <summary>
/// Class representing a menu option with its type, screen, and button.
/// </summary>
[System.Serializable]
public class MenuOption
{
    public MenuOptionType optionType; // The type of menu option
    public GameObject optionScreen; // The GameObject representing the option screen
    public Button optionButton; // The button associated with this option

    public bool ShowAsDefault = false; // Flag indicating if this option should be shown by default
}
