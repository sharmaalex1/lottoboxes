using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//enum of available Sounds Effects to play
public enum SFXType
{
    BoxAppearsOnScreen,
    BoxItem,
    BoxTap,
    BuffaloStampede,
    Earthquake,
    Flood,
    GainGoldenTicket,
    GainNothing,
    GoldenTicketAnimation,
    HandsGrabBox,
    HandSlapsBox,
    Hurricane,
    LoanScreenAppears,
    MultiplierIncrease,
    MultiplierReset,
    PlayerGrabsBox,
    PointGain,
    SharkAppears,
    SharkStealsBoxes,
    Tornado,
    UIInteraction,
    Wheel,
};
//enum of available Background music Tracks to play
public enum BGMType
{
    HighClass,
    LowClass,
    Working,
};
//enum of available Ambient music tracks to play
public enum AMBType
{
    ConveyerBelt,
    HighClassAmbience,
    LowClassAmbience,
};

/*This class is a persistent Manager that handles  all Audio In the game
 * All Audio Files can be found in the resources folder
 * They are separated Into Backgroud, Sound Effect, and Ambient sounds
 * If you have a new Background or Ambient Sound add it to the corresponding folder
 * Then put the EXACT name of that file into the corresponding Enumeration above
 * The name of the file and the enum MUST MATCH
 * The same goes for sound effects, but all sound effects are place in folders
 * The name of those folders should Match what is in the sound effect enumeration
 * The reason they are in folders is because we have multiple sound effects for the same action 
 * So we can randomly choose the sound
 */ 
public class AudioManager : MonoBehaviour
{
    #region Singleton Setup

    //private instance var for singleton
    private static AudioManager instance;

    //readonly property to allow access to instance
    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
        private set { }
    }

    public int boxGetIndex;

    //private constructor to stop other scripts form making an instance
    //Reason: Awake/Start doesn't run unless new instance is instantiated into the scene
    private AudioManager()
    {
    }

    #endregion

    #region Variables

    //audio data pulled from SaveManager
    private AudioSaveData audioData;
    //master audio listener
    private AudioListener audioListener;

    //audio source and clip dictionary for sound effects
    private AudioSource sfxAudioSource;
    public Dictionary<SFXType, AudioClip[]> sfxAudioClipDictionary;

    //audio source and clip dictionary for background music
    private AudioSource bgmAudioSource;
    public Dictionary<BGMType, AudioClip> bgmAudioClipDictionary;

    //audio source and clip dictionary for ambient music
    private AudioSource ambAudioSource;
    public Dictionary<AMBType, AudioClip> ambAudioClipDictionary;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        //regular singleton checks
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            //grab audio data from SaveManager
            audioData = SaveManager.Instance.GetAudioData();
            //set audio sources settings
            SetAudioSources();
            //set dicionaries
            SetDictionaries();
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }

    }

    //grab new audio listener whenever a scene is changed
    private void OnSceneWasLoaded()
    {
        audioListener = Camera.main.gameObject.GetComponent<AudioListener>();
    }

    #endregion

    #region Awake Functions

    //Initializes Sources and AudioListener
    private void SetAudioSources()
    {
        //set audio listener
        audioListener = GameObject.FindObjectOfType<AudioListener>();

        ///set background audio source
        bgmAudioSource = this.gameObject.AddComponent<AudioSource>();
        bgmAudioSource.playOnAwake = false;
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = audioData.bgmVolume;
        bgmAudioSource.mute = audioData.bgmMuted;

        //set ambiance audio source
        ambAudioSource = this.gameObject.AddComponent<AudioSource>();
        ambAudioSource.playOnAwake = false;
        ambAudioSource.loop = true;
        ambAudioSource.volume = audioData.ambVolume;
        ambAudioSource.mute = audioData.ambMuted;

        //set sound effects audio source
        sfxAudioSource = this.gameObject.AddComponent<AudioSource>();
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.loop = false;
        sfxAudioSource.volume = audioData.sfxVolume;
        sfxAudioSource.mute = audioData.sfxMuted;
    }

    //set audio dictionaries
    private void SetDictionaries()
    {
        sfxAudioClipDictionary = new Dictionary<SFXType, AudioClip[]>();
        foreach (SFXType type in Enum.GetValues(typeof(SFXType)))
        {
            sfxAudioClipDictionary.Add(type, Resources.LoadAll<AudioClip>("Audio/SoundEffects/" + type.ToString()));
        }

        bgmAudioClipDictionary = new Dictionary<BGMType, AudioClip>();
        foreach (BGMType type in Enum.GetValues(typeof(BGMType)))
        {
            bgmAudioClipDictionary.Add(type, Resources.Load<AudioClip>("Audio/Background/" + type.ToString()));
        }

        ambAudioClipDictionary = new Dictionary<AMBType, AudioClip>();
        foreach (AMBType type in Enum.GetValues(typeof(AMBType)))
        {
            ambAudioClipDictionary.Add(type, Resources.Load<AudioClip>("Audio/Ambient/" + type.ToString()));
        }
    }

    #endregion

    #region Mute

    /// <summary>
    /// Set if sounds effects are muted or not
    /// </summary>
    public void ToggleMuteSFXAudio(bool _mute)
    {
        audioData.sfxMuted = _mute;
        sfxAudioSource.mute = _mute;
    }

    /// <summary>
    /// Are sound effects muted?
    /// </summary>
    public bool IsSFXMuted()
    {
        return audioData.sfxMuted;
    }

    /// <summary>
    /// Set if ambient sounds are muted or not
    /// </summary>
    public void ToggleMuteAMBAudio(bool _mute)
    {
        audioData.ambMuted = _mute;
        ambAudioSource.mute = _mute;
    }

    /// <summary>
    /// are ambient sounds muted?
    /// </summary>
    public bool IsAMBMuted()
    {
        return audioData.ambMuted;
    }

    /// <summary>
    /// Set whether Background tracks are muted or not
    /// </summary
    public void ToggleMuteBGMAudio(bool _mute)
    {
        audioData.bgmMuted = _mute;
        bgmAudioSource.mute = _mute;
    }

    /// <summary>
    /// Are Background Tracks muted?
    /// </summary>
    public bool IsBGMMuted()
    {
        return audioData.bgmMuted;
    }

    #endregion

    #region Volume

    /// <summary>
    /// Set new Volume for Sound Effects
    /// </summary>
    public void SetSFXVolume(float _volume)
    {
        //clamp volume between 0 and 1
        _volume = Mathf.Clamp01(_volume);
        audioData.sfxVolume = _volume;
        sfxAudioSource.volume = _volume;
    }

    /// <summary>
    /// grab volume for sounds effects
    /// </summary>
    public float GetSFXVolume()
    {
        return audioData.sfxVolume;
    }

    /// <summary>
    ///  Set new volume for ambient sounds
    /// </summary
    public void SetAMBVolume(float _volume)
    {
        //clamp volume between 0 and 1
        _volume = Mathf.Clamp01(_volume);
        audioData.ambVolume = _volume;
        ambAudioSource.volume = _volume;
    }

    /// <summary>
    /// grab ambient sound volume
    /// </summary>
    public float GetAMBVolume()
    {
        return audioData.ambVolume;
    }

    /// <summary>
    /// Set new volume for Background Music
    /// </summary>
    public void SetBGMVolume(float _volume)
    {
        //clamp volume between 0 and 1 
        _volume = Mathf.Clamp01(_volume);
        audioData.bgmVolume = _volume;
        bgmAudioSource.volume = _volume;
    }

    /// <summary>
    /// grab volume for background music
    /// </summary>
    public float GetBGMVolume()
    {
        return audioData.ambVolume;
    }

    #endregion

    #region PlayAudioClip() Overrides

    /// <summary>
    /// Plays sound effect once given an index
    /// If no index is given, then a random sound effect from that folder
    /// </summary>
    public void PlayAudioClip(SFXType sfxToPlay, float volume = 1f, int index = -1)
    {
        AudioClip[] clipsToPlay;
        sfxAudioClipDictionary.TryGetValue(sfxToPlay, out clipsToPlay);
        if (index == -1)
        {
            sfxAudioSource.PlayOneShot(clipsToPlay[UnityEngine.Random.Range(0, clipsToPlay.Length)], volume);
        }
        else
        {
            sfxAudioSource.PlayOneShot(clipsToPlay[index], volume);
        }
    }

    /// <summary>
    /// Play BackGround Music on loop given Background music type
    /// </summary>
    public void PlayAudioClip(BGMType bgmToPlay, float volume = 1f)
    {
        AudioClip musicToPlay;
        bgmAudioClipDictionary.TryGetValue(bgmToPlay, out musicToPlay);
        //only play the clip if the current clip isn't the clip wanted OR if there is no background music playing
        if (bgmAudioSource.clip != musicToPlay || !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.clip = musicToPlay;
            bgmAudioSource.volume = GetBGMVolume() * volume;
            bgmAudioSource.Play();
        }
    }

    /// <summary>
    /// Play Ambient Music on loop given ambient music type
    /// </summary>
    public void PlayAudioClip(AMBType ambToPlay, float volume = 1f)
    {
        AudioClip musicToPlay;
        ambAudioClipDictionary.TryGetValue(ambToPlay, out musicToPlay);
        //only play the clip if the current clip isn't the clip wanted OR if there is no Ambient music playing
        if (ambAudioSource.clip != musicToPlay || !ambAudioSource.isPlaying)
        {
            ambAudioSource.clip = musicToPlay;
            ambAudioSource.volume = GetAMBVolume() * volume;
            ambAudioSource.Play();
        }
    }

    /// <summary>
    /// plays given audioclip
    /// Specify is this is a sound effect or not
    /// </summary>
    public void PlayAudioClip(AudioClip clipToPlay, bool isSFX, float volume = 1f)
    {
        //if it is a sound effect, play like normal
        if (isSFX)
        {
            sfxAudioSource.PlayOneShot(clipToPlay, volume);
        }
        else
        {
            //make an array to store background music clips
            AudioClip[] bgmClips = new AudioClip[bgmAudioClipDictionary.Count];
            //store background audioclips into array from dictionary
            bgmAudioClipDictionary.Values.CopyTo(bgmClips, 0);
            
            //iterate through background music clips
            for (int i = 0; i < bgmClips.Length; i++)
            {
                //if we find that the clip we want to play is in the background music clips
                //only play if the current Background clip is different than the clip to play OR there is no background music
                if (bgmClips[i] == clipToPlay && bgmAudioSource.clip != clipToPlay)
                {
                    bgmAudioSource.clip = clipToPlay;
                    bgmAudioSource.volume = GetBGMVolume() * volume;
                    bgmAudioSource.Play();   
                    return;
                }
            }

            //if we reach this point, the clip to play isn't a background clip
            //thus it is an ambient clip
            //only play clip if the current clip is different than the clip wanted OR there is no ambient music
            if (ambAudioSource.clip != clipToPlay)
            {
                ambAudioSource.clip = clipToPlay;
                ambAudioSource.volume = GetAMBVolume() * volume;
                ambAudioSource.Play();
            }   
        }
    }

    /// <summary>
    /// Plays a random audio clip given an array of audio clips.
    /// ONLY MEANT FOR SOUND EFFECTS
    /// </summary>
    public void PlayAudioClip(AudioClip[] clips, float volume = 1)
    {
        sfxAudioSource.PlayOneShot(clips[UnityEngine.Random.Range(0, clips.Length)], volume);
    }

    #endregion

    //Stops The Background sound if the Background sound given is the one that is being played
    public void StopAudioClip(BGMType bgmToStop)
    {
        AudioClip musicToStop;
        bgmAudioClipDictionary.TryGetValue(bgmToStop, out musicToStop);
        if (bgmAudioSource.clip == musicToStop)
        {
            bgmAudioSource.Stop();
        }
    }

    //Stops The Ambient sound if the Ambient sound given is the one that is being played
    public void StopAudioClip(AMBType ambToStop)
    {
        AudioClip musicToStop;
        ambAudioClipDictionary.TryGetValue(ambToStop, out musicToStop);
        if (ambAudioSource.clip == musicToStop)
        {
            ambAudioSource.Stop();
        }
    }
}
