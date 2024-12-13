using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private MusicType _backgroundMusic;

    private void Start()
    {
        AudioManager.instance.PlayAudio(_backgroundMusic);
    }
}
