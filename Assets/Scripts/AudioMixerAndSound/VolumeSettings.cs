using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer Mixer;
    public Slider Music, Sound, Ambience,Master;
    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_SFX = "SfxVolume";
    public const string MIXER_AMBIENCE = "AmbienceVolume";
    public const string MIXER_MASTER = "MasterVolume";
    private void Awake()
    {
        Master.onValueChanged.AddListener(SetMasterVolume);
        Music.onValueChanged.AddListener(SetMusicVolume);

        Sound.onValueChanged.AddListener(SetSfxVolume);

        Ambience.onValueChanged.AddListener(SetAmbienceVolume);

        UpdateSliderValues();
    }
    private void OnEnable()
    {
        //UpdateSliderValues();
    }
    //Volume settings region contains methods related to music,sfx and ambience settings ,save and load settings
    #region Volume Settings
    public void SetMusicVolume(float val)
    {
        Mixer.SetFloat(MIXER_MUSIC,Mathf.Log10(val) * 20);
    }
    public void SetMasterVolume(float val)
    {
        Mixer.SetFloat(MIXER_MASTER,Mathf.Log10(val) * 20);
    }
    public void SetSfxVolume(float val)
    {
        Mixer.SetFloat(MIXER_SFX,Mathf.Log10(val) * 20);
    }
    public void SetAmbienceVolume(float val)
    {
        Mixer.SetFloat(MIXER_AMBIENCE,Mathf.Log10(val) * 20);
    }
    public void UpdateSliderValues() 
    {
       
        float musicVolume = PlayerPrefs.GetFloat(AudioManager.MusicKey,1);

        float sfxVolume = PlayerPrefs.GetFloat(AudioManager.SfxKey,1);

        float ambienceVolume = PlayerPrefs.GetFloat(AudioManager.AmbienceKey,1);
        float masterVolume = PlayerPrefs.GetFloat(AudioManager.MasterKey,1);
        Music.value = musicVolume;
        Sound.value = sfxVolume;
        Ambience.value = ambienceVolume;
        Master.value = masterVolume;
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume); 
        SetAmbienceVolume(ambienceVolume);
    }

    #endregion
    private void OnDisable()
    {
        SaveVolumeSettings();
    }
    public void SaveVolumeSettings() 
    {
        PlayerPrefs.SetFloat(AudioManager.MusicKey,Music.value);

        PlayerPrefs.SetFloat(AudioManager.SfxKey,Sound.value);

        PlayerPrefs.SetFloat(AudioManager.AmbienceKey,Ambience.value);
        PlayerPrefs.SetFloat(AudioManager.MasterKey,Master.value);
    }
}
