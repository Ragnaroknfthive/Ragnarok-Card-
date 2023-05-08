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
