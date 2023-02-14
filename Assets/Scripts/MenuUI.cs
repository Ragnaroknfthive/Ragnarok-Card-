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
    public TMP_Dropdown dropdown;
    public Text regionTXT;
    public List<string> regions = new List<string>() { "Asia", "Australia", "Canada, East", "Europe", "India", "Japan", "Russia", "South America", "South Korea", "USA, East", "USA, West", "Russia, East", "South Africa", "Turkey" };
    static string AppID = "b155e53a-8156-43d7-ab29-461afb3885bb";


    private void Awake()
    {
        mui = this;
        PlayBtn.GetComponentInChildren<Text>().text = "Connecting...";
        regionTXT.text = "Connecting to...";
        PlayBtn.interactable = false;
        dropdown.interactable = false;
        InitOptions();
    }

    public void InitOptions()
    {
        List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
        foreach (var item in regions)
        {
            optionDatas.Add(new TMP_Dropdown.OptionData(item));
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
        PlayBtn.GetComponentInChildren<Text>().text = "Connecting...";
        regionTXT.text = "Connecting to...";
        PlayBtn.interactable = false;
        dropdown.interactable = false;

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
        LeanTween.alphaCanvas(Logo.GetComponent<CanvasGroup>(), 1f, 0.7f).setDelay(0.3f).setOnComplete(() =>
        {
            LeanTween.alphaCanvas(Logo.GetComponent<CanvasGroup>(), 0f, 0.3f).setDelay(0.7f);
            LeanTween.alphaCanvas(Splash.GetComponent<CanvasGroup>(), 0f, 0.3f).setDelay(0.7f);
        });

    }

    public void HideMsg()
    {
        PhotonNetwork.LeaveRoom();
        ErrorDisp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowMsg(string msg, bool Cancellable = false, bool AutoDistruct = false)
    {
        MsgTxt.text = msg;
        ErrorDisp.SetActive(true);

        if (ErrorDisp.GetComponentInChildren<Button>() != null)
            ErrorDisp.GetComponentInChildren<Button>().gameObject.SetActive(Cancellable);
        if (AutoDistruct)
        {
            StartCoroutine(Hide());
        }
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(3f);

        ErrorDisp.gameObject.SetActive(false);

    }

    public void UpdatePlayButtonText()
    {
        dropdown.interactable = true;
        PlayBtn.GetComponentInChildren<Text>().text = "PLAY";
        regionTXT.text = "Connected to";
        PlayBtn.interactable = true;
    }
}
