using System;
using UnityEngine;

public class MusicClipFactory 
{
    [SerializeField] private AudioClip _sadMusic;
    [SerializeField] private AudioClip _happyMusic;

    public MusicClipFactory(AudioClip sadMusic, AudioClip happyMusic)
    {
        _sadMusic = sadMusic;
        _happyMusic = happyMusic;   
    }
    public AudioClip GetMusicClip(MusicType type)
    {
        if (type == MusicType.SadMusic)
        {
            return _sadMusic;
        }
        else if (type == MusicType.HappyMusic)
        {
            return _happyMusic;
        }
        else
        {
            throw new InvalidOperationException(nameof(type));
        }
    }
}
