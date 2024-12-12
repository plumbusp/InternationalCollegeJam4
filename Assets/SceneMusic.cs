using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private MusicType _backgroundMusic;
    private void Start()
    {
        AudioManager.instance.PlayAudio(_backgroundMusic);
    }
    void Update()
    {
        if (Input.anyKey)
        {
            AudioManager.instance.PlayAudio(SFXType.ClickSound);
        }

    }
}
