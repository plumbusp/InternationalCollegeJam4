using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuickRestart : MonoBehaviour
{
    [SerializeField] private SceneFader sceneFader;
    private Image buttonimage;
    private void Awake()
    {
        sceneFader.IsAboutToFadeIn += () => { gameObject.SetActive(false); };
        buttonimage = GetComponent<Image>();
        StartCoroutine(DelayAppear());
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            RestartGame();
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private IEnumerator DelayAppear()
    {
        buttonimage.enabled = false;
        yield return new WaitForSeconds(1.5f);
        buttonimage.enabled = true;
    }
}
