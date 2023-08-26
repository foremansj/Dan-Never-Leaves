using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    
    bool isPaused;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PauseGame();
    }

    private void PauseGame()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote) && !isPaused)
        {
            Time.timeScale = 0;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
        }

        else if(Input.GetKeyDown(KeyCode.BackQuote) && isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseMenu.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
    }

    public void OpenOptions()
    {

    }

    public void OpenHelp()
    {
        
    }

    public void QuitGame()
    {
        
    }
}
