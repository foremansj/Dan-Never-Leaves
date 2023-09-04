using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("First Floor");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void OpenHowToPlay()
    {
        SceneManager.LoadScene("How To Play");
    }

    public void OpenStartMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }

    public void OpenIntroduction()
    {
        SceneManager.LoadScene("Introduction");
    }
}
