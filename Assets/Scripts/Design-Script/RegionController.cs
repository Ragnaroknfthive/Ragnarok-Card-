////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: RegionController.cs
//FileType: C# Source file
//Description : This script is used to select manual photon network region for PVP mode
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RegionController : MonoBehaviour
{
    private static RegionController rc;                 //Script instance                

    public Dropdown dropdown;                           //Region list dropdown
    public List<string> regions = new List<string>() { "Asia", "Australia", "Canada, East", "Europe", "India", "Japan", "Russia", "South America", "South Korea", "USA, East", "USA, West", "Russia, East", "South Africa", "Turkey" };
    //Photon network regions

    /// <summary>
    /// Set instance and initialize region options
    /// </summary>
    private void Awake()
    {
        rc = this;
        dropdown.interactable = true;
        InitOptions();
    }
    /// <summary>
    /// Fill region dropdown list and set current selected region
    /// </summary>
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
    /// <summary>
    /// Get string value for current selected photon region
    /// </summary>
    /// <returns></returns>
    public string getSelectedRegion()
    {
        return GetRegionCode(PlayerPrefs.GetInt("pun_region", 0));
    }

    /// <summary>
    /// This function returns string code for given intpu regoin as interger value
    /// </summary>
    /// <param name="s">Region integer value</param>
    /// <returns>Strign code text for region</returns>
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
    /// <summary>
    /// Set region with respect to index
    /// </summary>
    /// <param name="regionIndex">Region index</param>
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
    /// <summary>
    /// Get index of region using input string value of region
    /// </summary>
    /// <param name="_region">region code</param>
    /// <returns>region index</returns>
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
    /// <summary>
    /// Update dropdown selected text;
    /// </summary>
    public void UpdateDropdownText()
    {
        int selectedIndex = dropdown.value;

        if (selectedIndex >= 0 && selectedIndex < regions.Count)
        {
            dropdown.captionText.text = regions[selectedIndex];
        }
    }
    /// <summary>
    /// Trigger new region connection on photon network
    /// </summary>
    public void OnSelectRegion()
    {
        string region = getSelectedRegion();
        StartCoroutine(ConnnectToNewRegion(region));
    }
    /// <summary>
    /// Disconnect from photon network and connect to new region
    /// </summary>
    /// <param name="region">new region code</param>
    public static IEnumerator ConnnectToNewRegion(string region)
{
    PhotonNetwork.Disconnect();
    yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);
    PhotonNetwork.ConnectToRegion(region);
    PlayerPrefs.SetInt("pun_region", rc.dropdown.value);
}
    /// <summary>
    /// Get region conntroller instnace
    /// </summary>
    /// <returns></returns>
    public static RegionController Get()
    {
        return rc;
    }
}
