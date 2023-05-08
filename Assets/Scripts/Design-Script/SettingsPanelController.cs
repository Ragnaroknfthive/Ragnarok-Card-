using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button settingsButton;
    public Button closeButton;

    private void Start()
    {
        settingsButton.onClick.AddListener(ToggleSettingsPanel);
        closeButton.onClick.AddListener(CloseSettingsPanel);
    }

    private void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
}
