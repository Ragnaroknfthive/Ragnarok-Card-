////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: SettingsPanelController.cs
//FileType: C# Source file
//Description : This script handles settings panel related logic
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    public GameObject settingsPanel;                                    //Settings panel reference object
    public Button settingsButton;                                       //Settings button reference
    public Button closeButton;                                          //Panel Close button referene
    public List<SettingsOption> settingsOptions = new List<SettingsOption>();  //list of setting options
    /// <summary>
    /// Add click event listners and show sound settings option
    /// </summary>
    private void Start()
    {
        settingsButton.onClick.AddListener(ToggleSettingsPanel);
        closeButton.onClick.AddListener(CloseSettingsPanel);
        ShowSoundSettings();
    }
    /// <summary>
    /// Toggle settings panel gameobject
    /// </summary>
    private void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    /// <summary>
    /// Close settings panel
    /// </summary>
    private void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
    /// <summary>
    /// Update graphic quality with respect to input index
    /// </summary>
    /// <param name="index">Graphics quality index</param>
    public void Update_GraphicsQualitySettings(int index) 
    {
        QualitySettings.SetQualityLevel(index);
    }
    /// <summary>
    /// Show graphics related settings option
    /// </summary>
    public void ShowGraphicsSettings() 
    {
        OpenSettingOption(SettingsOptionType.Graphics);
    }
    /// <summary>
    /// Show sounds related settings option
    /// </summary>
    public void ShowSoundSettings()
    {
        OpenSettingOption(SettingsOptionType.Sound);
    }
    /// <summary>
    /// Show gameplay settings option
    /// </summary>
    public void ShowGameplaySettings()
    {
        OpenSettingOption(SettingsOptionType.Gameplay);
    }
    /// <summary>
    /// Other settings option display
    /// </summary>
    public void ShowOtherSettings()
    {
        OpenSettingOption(SettingsOptionType.Other);
    }
    /// <summary>
    /// This function shows respective settings option screen with respect to input setting option type
    /// </summary>
    /// <param name="optionType">Setting Option type</param>
    public void OpenSettingOption(SettingsOptionType optionType) 
    {
        foreach(SettingsOption item in settingsOptions)
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
/// <summary>
/// Settings option type (sounds, graphic, gameplay etc..)
/// </summary>
[System.Serializable]
public enum SettingsOptionType {Sound,Graphics,Gameplay,Other }
/// <summary>
/// This class is used to defince a settings option and hold object references for respective setting option
/// </summary>
[System.Serializable]
public class SettingsOption
{
    public SettingsOptionType optionType;
    public GameObject optionScreen;
    public Button optionButton;

    public bool ShowAsDefault = false;
}