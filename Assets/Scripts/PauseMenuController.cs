using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject resumeButton;
    //[SerializeField] GameObject clearHandButton;
    [SerializeField] GameObject controlsButton;
    //[SerializeField] GameObject startMenuButton;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject quitButton;
    //[SerializeField] GameObject optionsButton;

    [SerializeField] Image keyboardControlsImage;
    [SerializeField] GameObject backButton;
    
    bool isPaused;
    
    private void Awake()
    {
        Camera.main.GetComponent<AudioSource>().Pause();
        isPaused = true;
        Time.timeScale = 0;
    }

    void Update()
    {
        PauseGame();
        if(isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void PauseGame()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote) && !isPaused)
        {
            Time.timeScale = 0;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            Camera.main.GetComponent<AudioSource>().Pause();
        }

        else if(Input.GetKeyDown(KeyCode.BackQuote) && isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseMenu.SetActive(false);
            Camera.main.GetComponent<AudioSource>().Play();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Camera.main.GetComponent<AudioSource>().Play();
    }

    public void ViewControls()
    {
        resumeButton.SetActive(false);
        //clearHandButton.SetActive(false);
        controlsButton.SetActive(false);
        //startMenuButton.SetActive(false);
        restartButton.SetActive(false);
        quitButton.SetActive(false);
        //optionsButton.SetActive(false);

        keyboardControlsImage.enabled = true;
        backButton.SetActive(true);
    }
    public void BackToPauseMenu()
    {
        resumeButton.SetActive(true);
        //clearHandButton.SetActive(true);
        controlsButton.SetActive(true);
        //startMenuButton.SetActive(true);
        restartButton.SetActive(true);
        quitButton.SetActive(true);
        //optionsButton.SetActive(true);

        keyboardControlsImage.enabled = false;
        backButton.SetActive(false);
    }
}
