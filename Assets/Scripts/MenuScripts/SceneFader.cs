using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public event Action IsAboutToFadeIn;

    private void Awake()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger("FadeOut");
    }
    public void FadeIn()
    {
        IsAboutToFadeIn?.Invoke();
        gameObject.SetActive(true);
        _animator.SetTrigger("FadeIn");
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
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
