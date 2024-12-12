using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlayAudio(MusicType.MainMenu);
    }
    void Update()
    {
        if (Input.anyKey)
        {
            AudioManager.instance.PlayAudio(SFXType.ClickSound);
        }

    }
}
