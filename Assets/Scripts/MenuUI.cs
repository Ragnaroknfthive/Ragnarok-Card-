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

    private static MenuUI mui;

    public GameObject ErrorDisp;

    private PhotonView photonView;

    public Text MsgTxt;
    public Button PlayBtn;

    public GameObject Logo, Splash;
  //  public TMP_Dropdown dropdown;
    public Dropdown dropdown;
    public Text regionTXT;
    public List<string> regions = new List<string>() { "Asia", "Australia", "Canada, East", "Europe", "India", "Japan", "Russia", "South America", "South Korea", "USA, East", "USA, West", "Russia, East", "South Africa", "Turkey" };
    static string AppID = "b155e53a-8156-43d7-ab29-461afb3885bb";
    public List<MenuOption> menuOptions = new List<MenuOption>();
    public Sprite deckCardHoverSprite,deckHoverSprite;

    private void Awake()
    {
        mui = this;
       // if(PlayBtn)
       // PlayBtn.GetComponentInChildren<Text>().text = "Connecting...";
        if(regionTXT)
        regionTXT.text = "Connecting to...";
        if(PlayBtn)
        PlayBtn.interactable = false;
        if(dropdown)
        dropdown.interactable = false;
        InitOptions();
    }

    public void InitOptions()
    {
        if(!dropdown) return;

        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        foreach (var item in regions)
        {
            optionDatas.Add(new Dropdown.OptionData(item));
        }
        dropdown.options = optionDatas;
        dropdown.value = PlayerPrefs.GetInt("pun_region", 0);
    }

    public string getSelectedRegion()
    {
        return GetRegionCode(PlayerPrefs.GetInt("pun_region", 0));
    }


    public string GetRegionCode(int s)
    {
        string region = "us";
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

    public void OnSelectRegion()
    {
        string region = "us";
        switch ((dropdown.value))
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
       // if(PlayBtn)
       // PlayBtn.GetComponentInChildren<Text>().text = "Connecting...";
        if(regionTXT)
        regionTXT.text = "Connecting to...";
        if(PlayBtn)
        PlayBtn.interactable = false;
        if(dropdown)
        dropdown.interactable = false;

        if(dropdown)
        StartCoroutine(ConnnectToNewRegion(region));


    }

    IEnumerator ConnnectToNewRegion(string region)
    {
        PhotonNetwork.Disconnect();
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);
        Debug.Log("here here");
        PhotonNetwork.ConnectToRegion(region);
        PlayerPrefs.SetInt("pun_region", dropdown.value);
    }

    public static MenuUI Get()
    {
        return mui;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(menuOptions.Count > 0) 
        {
            ShowHomeScreen();
        }
        if(!Logo) return;

        LeanTween.alphaCanvas(Logo.GetComponent<CanvasGroup>(), 1f, 0.7f).setDelay(0.3f).setOnComplete(() =>
        {
            LeanTween.alphaCanvas(Logo.GetComponent<CanvasGroup>(), 0f, 0.3f).setDelay(0.7f);
            LeanTween.alphaCanvas(Splash.GetComponent<CanvasGroup>(), 0f, 0.3f).setDelay(0.7f);
        });

    }

    public void HideMsg()
    {
        PhotonNetwork.LeaveRoom();
       // PhotonNetwork.Disconnect();
        ErrorDisp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if(!PhotonNetwork.IsConnected) 
        //{
        //    PlayBtn.GetComponentInChildren<Text>().text = "Connecting...";
        //    regionTXT.text = "Connecting to...";
        //    PlayBtn.interactable = false;
        //    dropdown.interactable = false;
        //}

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

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(3f);

        ErrorDisp.gameObject.SetActive(false);

    }

    public void UpdatePlayButtonText()
    {
        if(!PlayBtn) return;

        dropdown.interactable = true;
       // PlayBtn.GetComponentInChildren<Text>().text = "PLAY";
        regionTXT.text = "Connected to";
        PlayBtn.interactable = true;
    }
    public void Quit() 
    {
        Application.Quit();
    }
    public void ShowHomeScreen()
    {
        OpenMenuOption(MenuOptionType.Home);
    }
    public void ShowPlayScreen()
    {
        OpenMenuOption(MenuOptionType.Play);
    }
    public void ShowPRofileScreen()
    {
        OpenMenuOption(MenuOptionType.Profile);
    }
    public void ShowDeckScreen()
    {
        OpenMenuOption(MenuOptionType.Decks);
    }
    public void ShowShopScreen()
    {
        OpenMenuOption(MenuOptionType.Shop);
    }
    public void OpenMenuOption(MenuOptionType optionType)
    {
        foreach(MenuOption item in menuOptions)
        {
            if(item.optionType == optionType)
            {
                item.optionScreen.SetActive(true);

            }
            else
            {
                item.optionScreen.SetActive(false);
                //  item.optionButton.interactable = true;
            }
        }
    }
}

[System.Serializable]
public enum MenuOptionType { Home,Play,Profile, Decks, Shop }
[System.Serializable]
public class MenuOption
{
    public MenuOptionType optionType;
    public GameObject optionScreen;
    public Button optionButton;

    public bool ShowAsDefault = false;
}
