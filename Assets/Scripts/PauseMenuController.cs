using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
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
    [SerializeField] GameObject optionsButton;

    [SerializeField] Image keyboardControlsImage;
    [SerializeField] GameObject backButton;
    
    [Header("Mouse Sensitivity")]
    [SerializeField] GameObject mouseSensitivityPanel;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] TextMeshProUGUI sensitivityValueText;
    [SerializeField] CinemachineVirtualCamera firstPersonCamera; 
    public float sensitivityFloor = 50f;
    public float sensitivityCeiling = 300f;
    
    UIController uIController;
    bool isPaused;
    
    private void Awake()
    {
        Camera.main.GetComponent<AudioSource>().Pause();
        isPaused = true;
        Time.timeScale = 0;
    }

    private void Start()
    {
        uIController = GetComponent<UIController>();
    }

    void Update()
    {
        PauseGame();
        if(isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            SetMouseSensitivity();
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
            uIController.cameraReticle.SetActive(false);
        }

        else if(Input.GetKeyDown(KeyCode.BackQuote) && isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseMenu.SetActive(false);
            Camera.main.GetComponent<AudioSource>().Play();
            uIController.cameraReticle.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Camera.main.GetComponent<AudioSource>().Play();
        uIController.cameraReticle.SetActive(true);
    }

    public void ViewControls()
    {
        resumeButton.SetActive(false);
        //clearHandButton.SetActive(false);
        controlsButton.SetActive(false);
        restartButton.SetActive(false);
        quitButton.SetActive(false);
        optionsButton.SetActive(false);
        mouseSensitivityPanel.SetActive(false);

        keyboardControlsImage.enabled = true;
        backButton.SetActive(true);
    }
    public void BackToPauseMenu()
    {
        resumeButton.SetActive(true);
        //clearHandButton.SetActive(true);
        controlsButton.SetActive(true);
        restartButton.SetActive(true);
        quitButton.SetActive(true);
        optionsButton.SetActive(true);
        mouseSensitivityPanel.SetActive(false);

        keyboardControlsImage.enabled = false;
        backButton.SetActive(false);
    }

    public void ViewOptions()
    {
        resumeButton.SetActive(false);
        //clearHandButton.SetActive(false);
        controlsButton.SetActive(false);
        restartButton.SetActive(false);
        quitButton.SetActive(false);
        optionsButton.SetActive(false);
        

        mouseSensitivityPanel.SetActive(true);
        backButton.SetActive(true);
    }

    public void SetMouseSensitivity()
    {
        mouseSensitivitySlider.minValue = sensitivityFloor;
        mouseSensitivitySlider.maxValue = sensitivityCeiling;
        float sliderPercent = Mathf.Round((mouseSensitivitySlider.value / sensitivityCeiling) * 100);
        sensitivityValueText.text = sliderPercent.ToString();
        firstPersonCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = mouseSensitivitySlider.value * 0.5f;
        firstPersonCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = mouseSensitivitySlider.value;
    }
}
