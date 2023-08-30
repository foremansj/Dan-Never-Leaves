using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LevelManager : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 2f;
    ScoreKeeper scoreKeeper;
    PlayerInput playerInput;


    private void Awake()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        playerInput = FindObjectOfType<PlayerInput>();
    }
    
    public void RestartGame()
    {
        scoreKeeper.ResetScore();
        SceneManager.LoadScene("First Floor");
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene("Start Menu");
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadGameOver()
    {
        StartCoroutine(WaitAndLoad("Game Over", sceneLoadDelay));
        Cursor.lockState = CursorLockMode.None;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
