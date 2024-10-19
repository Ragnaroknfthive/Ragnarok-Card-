using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer Mixer;
  
    public const string MIXER_MUSIC= "MusicVolume";
    public const string MIXER_SFX = "SfxVolume";
    public const string MIXER_AMBIENCE = "AmbienceVolume";
    public const string MIXER_Master = "MasterVolume";

    public const string MusicKey = "musicVolume";

    public const string SfxKey = "sfxVolume";

    public const string AmbienceKey = "ambienceVolume";
    public const string MasterKey = "masterVolume";
    public static AudioManager instance;
    private void Awake()
    {
        if(instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        LoadVolume(); 
    }
    public void LoadVolume()//Vlumen settings saved from VolumeSettings.cs
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterKey,1);

        float musicVolume = PlayerPrefs.GetFloat(MusicKey,1);

        float sfxVolume = PlayerPrefs.GetFloat(SfxKey,1);

        float ambienceVolume = PlayerPrefs.GetFloat(AmbienceKey,1);
        Mixer.SetFloat(MIXER_MUSIC,Mathf.Log10( musicVolume)*20);
        Mixer.SetFloat(MIXER_SFX,Mathf.Log10(sfxVolume) * 20);
        Mixer.SetFloat(MIXER_AMBIENCE,Mathf.Log10(ambienceVolume) * 20);
        Mixer.SetFloat(MIXER_Master,Mathf.Log10(masterVolume) * 20);
    }
    
}
