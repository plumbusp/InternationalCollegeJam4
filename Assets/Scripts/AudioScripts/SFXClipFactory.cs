
using UnityEngine;
using System;

public class SFXClipFactory 
{
    private AudioClip _shootSound;
    private AudioClip _clickSound;
    public SFXClipFactory(AudioClip shootSound, AudioClip clickSound)
    {
        _shootSound = shootSound;
        _clickSound = clickSound;
    }
    public AudioClip GetSFXClip(SFXType type)
    {
        if(type == SFXType.ShootSound)
        {
            return _shootSound;
        }
        else if(type == SFXType.ClickSound)
        {
            return _clickSound;
        }
        else
        {
            throw new InvalidOperationException(nameof(type));
        }
    }
}
