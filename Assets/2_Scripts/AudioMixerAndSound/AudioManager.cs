////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: AudioManager.cs
//FileType: C# Source file
//Description : This script is used to handle audio ,sfx ,ambience key's and load respective volumes
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer Mixer;
  
    public const string MIXER_MUSIC= "MusicVolume";             //Used to save/load music volume
    public const string MIXER_SFX = "SfxVolume";                //Used to save/load sfx volume
    public const string MIXER_AMBIENCE = "AmbienceVolume";      //Used to save/load ambience
    public const string MIXER_Master = "MasterVolume";          //Used to save/load Master volume of mixer

    public const string MusicKey = "musicVolume";               //key for volume

    public const string SfxKey = "sfxVolume";                   //key for volume

    public const string AmbienceKey = "ambienceVolume";         //key for volume
    public const string MasterKey = "masterVolume";             //key for volume
    public static AudioManager instance;                        //Instance for this script
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
    /// <summary>
    /// Trigger Load mixer volumes function
    /// </summary>
    private void Start()
    {
        LoadVolume(); 
    }
    /// <summary>
    /// Load mixer volumes 
    /// </summary>
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
