using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Objects")]
    [SerializeField] private List<MusicObject> musicObjects = new List<MusicObject>();
    [SerializeField] private List<SFXObject> SFXObjects = new List<SFXObject>();

    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _SFXAudioSource;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _SFXSlider;

    private VolumeController _volumeController;

    private Dictionary<string, AudioClip> _audioClips;

    private void Initialize()
    {
        _volumeController = new VolumeController(_audioMixer, _musicSlider, _SFXSlider);
        _audioClips = new Dictionary<string, AudioClip>();
        foreach (var musicObject in musicObjects)
        {
            _audioClips.Add(musicObject.musicType.ToString(), musicObject.clip);
        }
        foreach (var SFXObject in SFXObjects)
        {
            _audioClips.Add(SFXObject.sfxType.ToString(), SFXObject.clip);
        }
    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayAudio(MusicType musicType)
    {
        try
        {
            _musicAudioSource.clip = _audioClips[musicType.ToString()];
            _musicAudioSource.Play();
        }

        catch (Exception e)
        {
            if (!_audioClips.ContainsKey(musicType.ToString()))
            {
                Debug.LogWarning("There is no value with " + musicType.ToString() + " key " + " in " + _audioClips);
            }
            else
            {
                throw new Exception(e.Message);
            }
        }
    }
    public void PlayAudio(SFXType soundType)
    {
         try
         {
             _SFXAudioSource.clip = _audioClips[soundType.ToString()];
             _SFXAudioSource.Play();
         }

         catch (Exception e)
         {
             if (!_audioClips.ContainsKey(soundType.ToString()))
             {
                  Debug.LogWarning("There is no value with " + soundType.ToString() + " key " + " in " + _audioClips);
             }
             else
             {
                  throw new Exception(e.Message);
             }
         }
    }
}
