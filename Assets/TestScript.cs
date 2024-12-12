using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlayAudio(MusicType.HappyMusic);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            AudioManager.instance.PlayAudio(SFXType.ClickSound);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AudioManager.instance.PlayAudio(SFXType.ShootSound);
        }
    }
}
