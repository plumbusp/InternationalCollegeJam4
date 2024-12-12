using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreensLogic : MonoBehaviour
{
    public static ScreensLogic Instance { get; private set; }

    [SerializeField] private GameObject deadScreenUI;
    [SerializeField] private GameObject _pauseMenu;

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;
        //Hide all panels
        deadScreenUI.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    public void ShowDeadScreen()
    {
        Time.timeScale = 0;
        deadScreenUI.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_pauseMenu.activeSelf)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    public void ClosePauseMenu()
    {
        _pauseMenu.SetActive(false);
        UnpauseGame();
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OpenPauseMenu()
    {
        _pauseMenu.SetActive(true);
        PauseGame();
    }

    private void UnpauseGame() => Time.timeScale = 1;

    private void PauseGame() => Time.timeScale = 0;
}
