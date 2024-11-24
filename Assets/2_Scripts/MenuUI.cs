///////////////////////////////////
/// MenuUI.cs
/// 
/// This script manages the user interface for the main menu,
/// including region selection, connecting to Photon servers, and navigating between different menu options.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Photon.Pun.UtilityScripts;

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

    /// Initializes the MenuUI instance and sets up the initial state of the UI.
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

    /// Initializes the dropdown options with the available regions.
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

    /// Gets the currently selected region code.
    /// <returns>Region code as a string.
    public string getSelectedRegion()
    {
        return GetRegionCode(PlayerPrefs.GetInt("pun_region", 0));
    }

    /// Maps the dropdown selection index to the corresponding region code.
    /// Index of the selected region.
    /// Region code as a string.
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

    /// Called when a new region is selected from the dropdown.
    /// Updates the UI and initiates a connection to the selected region.
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

    public static MenuUI Get()
    {
        return mui;
    }

    /// Coroutine to handle connecting to a new region.
    /// The region to connect to.
    /// Yield instruction to wait during the connection process.
    IEnumerator ConnnectToNewRegion(string region)
    {
        PhotonNetwork.Disconnect(); // Disconnect from the current server
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected); // Wait until disconnected

        //Debug.Log("Connecting to region: " + region);
        PhotonNetwork.ConnectToRegion(region); // Connect to the new region
        PlayerPrefs.SetInt("pun_region", dropdown.value); // Save selected region to PlayerPrefs
    }

    /// Initializes the menu UI on start.
    /// Displays the home screen if menu options are available.
    /// Fades in the logo and splash screen.
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

    /// Hides the error message display.
    /// Leaves the current Photon room and disables the error display.
    public void HideMsg()
    {
        PhotonNetwork.LeaveRoom(); // Leave the room if in one
        ErrorDisp.SetActive(false); // Disable the error display
    }

    /// Coroutine to hide the error display after a delay.
    /// Yield instruction to wait before hiding the display.
    IEnumerator Hide()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        ErrorDisp.gameObject.SetActive(false); // Disable the error display
    }

    /// Updates the play button text and enables the play button and dropdown after connecting.
    public void UpdatePlayButtonText()
    {
        if (!PlayBtn) return;

        dropdown.interactable = true; // Enable dropdown for region selection
        regionTXT.text = "Connected to"; // Update text to show connection status
        PlayBtn.interactable = true; // Enable the play button
    }
    public void ShowMsg(string msg, bool Cancellable = false, bool AutoDistruct = false)
    {
        //MsgTxt.text = msg;
        //ErrorDisp.SetActive(true);

        //if(ErrorDisp.GetComponentInChildren<Button>() != null)
        //    ErrorDisp.GetComponentInChildren<Button>().gameObject.SetActive(Cancellable);
        //if(AutoDistruct)
        //{
        //    StartCoroutine(Hide());
        //}
    }

    /// Quits the application.
    public void Quit()
    {
        Application.Quit(); // Exit the application
    }

    /// Displays the home screen by opening the corresponding menu option.
    public void ShowHomeScreen()
    {
        OpenMenuOption(MenuOptionType.Home); // Open the home menu option
    }

    /// Displays the play screen by opening the corresponding menu option.
    public void ShowPlayScreen()
    {
        OpenMenuOption(MenuOptionType.Play); // Open the play menu option
    }

    /// Displays the profile screen by opening the corresponding menu option.
    public void ShowPRofileScreen()
    {
        OpenMenuOption(MenuOptionType.Profile); // Open the profile menu option
    }

    /// Displays the deck screen by opening the corresponding menu option.
    public void ShowDeckScreen()
    {
        OpenMenuOption(MenuOptionType.Decks); // Open the decks menu option
    }

    /// Displays the shop screen by opening the corresponding menu option.
    public void ShowShopScreen()
    {
        OpenMenuOption(MenuOptionType.Shop); // Open the shop menu option
    }

    /// Opens the specified menu option and hides all others.
    /// The type of menu option to open.
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

/// Enum representing different menu option types.
[System.Serializable]
public enum MenuOptionType { Home, Play, Profile, Decks, Shop }

/// Class representing a menu option with its type, screen, and button.
[System.Serializable]
public class MenuOption
{
    public MenuOptionType optionType; // The type of menu option
    public GameObject optionScreen; // The GameObject representing the option screen
    public Button optionButton; // The button associated with this option

    public bool ShowAsDefault = false; // Flag indicating if this option should be shown by default
}
