using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RegionController : MonoBehaviour
{
    private static RegionController rc;

    public Dropdown dropdown;
    public List<string> regions = new List<string>() { "Asia", "Australia", "Canada, East", "Europe", "India", "Japan", "Russia", "South America", "South Korea", "USA, East", "USA, West", "Russia, East", "South Africa", "Turkey" };

    private void Awake()
    {
        rc = this;
        dropdown.interactable = true;
        InitOptions();
    }

    public void InitOptions()
    {
        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
        foreach (var item in regions)
        {
            optionDatas.Add(new Dropdown.OptionData(item));
        }
        dropdown.options = optionDatas;
        dropdown.value = PlayerPrefs.GetInt("pun_region", 0);
        UpdateDropdownText();
    }

    public string getSelectedRegion()
    {
        return GetRegionCode(PlayerPrefs.GetInt("pun_region", 0));
    }

    public static string GetRegionCode(int s)
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

    public void SetRegion(int regionIndex)
    {
        if (regionIndex >= 0 && regionIndex < regions.Count)
        {
            // Set the dropdown value to the selected region index
            dropdown.value = regionIndex;

            // Save the selected region in PlayerPrefs
            PlayerPrefs.SetInt("pun_region", regionIndex);

            // Update the dropdown text to show the selected region
            UpdateDropdownText();
        }
    }
    public static  int GetIndexOfRegion(string _region) 
    {
        int index = -1;
       _region= _region.Remove(_region.Length-2,2);
        switch(_region)
        {
            case "asia":index = 0; break;
            case "au":index=1; break;
            case "cae":index=2; break;
            case "eu": index=3;break;
            case "in":index=4; break;
            case "jp":index=5; break;
            case "ru":index=6; break;
            case "sa":index=7; break;
            case "kr":index=8; break;
            case "us":index=9; break;
            case "usw":index=10; break;
            case "rue":index=11; break;
            case "za":index=12; break;
            case "tr":index=13; break;
        }
        return index;
    }

    public void UpdateDropdownText()
    {
        int selectedIndex = dropdown.value;

        if (selectedIndex >= 0 && selectedIndex < regions.Count)
        {
            dropdown.captionText.text = regions[selectedIndex];
        }
    }

    public void OnSelectRegion()
    {
        string region = getSelectedRegion();
        StartCoroutine(ConnnectToNewRegion(region));
    }

    public static IEnumerator ConnnectToNewRegion(string region)
{
    PhotonNetwork.Disconnect();
    yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);
    PhotonNetwork.ConnectToRegion(region);
    PlayerPrefs.SetInt("pun_region", rc.dropdown.value);
}

    public static RegionController Get()
    {
        return rc;
    }
}
