using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController
{
    private AudioMixer _audioMixer;
    private Slider _musicSlider;
    private Slider _SFXSlider;

    const string MIXER_MUSIC = "MusicVolume";    // Exposed param of Music volume (in AudioMixer)
    const string MIXER_SFX = "SFXVolume";        // Exposed param of SFX volume (in AudioMixer)

    public VolumeController (AudioMixer audioMixer, Slider musicSlider, Slider SFXSlider)
    {
        _audioMixer = audioMixer;
        _musicSlider = musicSlider;
        _SFXSlider = SFXSlider;

        _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        _SFXSlider.onValueChanged.AddListener(ChangeSFXVolume);
    }
    public void DestroyObject()
    {
        _musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
        _SFXSlider.onValueChanged.RemoveListener(ChangeSFXVolume);
    }
    private void ChangeMusicVolume(float value)
    {
        Debug.Log("CHANGE MUCIS");
        _audioMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20f);
    }
    private void ChangeSFXVolume(float value)
    {
        _audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20f);
    }

}
