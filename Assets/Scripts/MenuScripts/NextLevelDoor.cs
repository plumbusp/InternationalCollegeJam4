using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelDoor : MonoBehaviour
{
    [SerializeField] private SceneFader _sceneFader;

    private void Awake()
    {
        if(_sceneFader == null)
        {
            Debug.LogError("NO SCENE FADER");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<HamsterMovement>().InSafeSpot = true;
            _sceneFader.FadeIn();
        }
    }
}