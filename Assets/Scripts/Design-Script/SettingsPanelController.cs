using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button settingsButton;
    public Button closeButton;
    public List<SettingsOption> settingsOptions = new List<SettingsOption>();

    private void Start()
    {
        settingsButton.onClick.AddListener(ToggleSettingsPanel);
        closeButton.onClick.AddListener(CloseSettingsPanel);
        ShowSoundSettings();
    }

    private void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
    public void Update_GraphicsQualitySettings(int index) 
    {
        QualitySettings.SetQualityLevel(index);
    }
    public void ShowGraphicsSettings() 
    {
        OpenSettingOption(SettingsOptionType.Graphics);
    }
    public void ShowSoundSettings()
    {
        OpenSettingOption(SettingsOptionType.Sound);
    }
    public void ShowGameplaySettings()
    {
        OpenSettingOption(SettingsOptionType.Gameplay);
    }
    public void ShowOtherSettings()
    {
        OpenSettingOption(SettingsOptionType.Other);
    }
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
[System.Serializable]
public enum SettingsOptionType {Sound,Graphics,Gameplay,Other }
[System.Serializable]
public class SettingsOption
{
    public SettingsOptionType optionType;
    public GameObject optionScreen;
    public Button optionButton;

    public bool ShowAsDefault = false;
}