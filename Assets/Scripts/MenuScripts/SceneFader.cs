using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public event Action IsAboutToFadeOut;
    public void FadeIn()
    {
        _animator.SetTrigger("FadeIn");
    }

    //Invokes righ after animation is played
    public void TransferToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void TransferToNextScene()
    {
        var nextIndex = (SceneManager.GetActiveScene().buildIndex) + 1;
        SceneManager.LoadScene(nextIndex);
    }
}
