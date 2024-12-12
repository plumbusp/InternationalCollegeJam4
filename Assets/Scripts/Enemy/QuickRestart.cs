using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuickRestart : MonoBehaviour
{
    private Image buttonimage;
    private void Awake()
    {
        GameObject.Find("SceneFader").GetComponent<SceneFader>().IsAboutToFadeOut += () => { gameObject.SetActive(false); };
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
        Debug.Log("Set enabled false");
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Set enabled true");
        buttonimage.enabled = true;
    }
}
