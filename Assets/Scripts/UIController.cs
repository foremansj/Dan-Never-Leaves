using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] public TextMeshProUGUI totalSalesText;
    [SerializeField] public TextMeshProUGUI totalTipsText;
    [SerializeField] public TextMeshProUGUI totalTipsPercentText;
    [SerializeField] GameObject StepsOfServiceInstructions;
    [SerializeField] TextMeshProUGUI stepsOfServiceReminder;
    [SerializeField] TextMeshProUGUI lastTableTouchedText;

    [SerializeField] float clockSpeed = 2f;
    float seconds;
    float minutes;
    float hours = 5f;

    PlayerInteraction playerInteraction;
    ScoreKeeper scoreKeeper;
    LevelManager levelManager;

    private void Awake()
    {
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        levelManager = FindObjectOfType<LevelManager>();
    }
    private void Start()
    {
        scoreKeeper.CallScore();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayLastTableTouched();
        ViewStepsOfService();
        GameTimer();
    }

    public void GameTimer()
    {
        seconds += Time.deltaTime * clockSpeed;
        if(seconds > 59.5f)
        {
            minutes += 1f;
            seconds = -0.4f;
        }
        if(minutes > 59.5f)
        {
            hours += 1;
            minutes = 0f;
        }
        timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds) + " PM";

        if(hours + (minutes + (seconds / 60)) / 60 >= 11.5)
        {
            levelManager.LoadGameOver();
        }
    }

    public float GetWorldTime()
    {
        return hours + (minutes + (seconds / 60)) / 60;
    }

    private void ViewStepsOfService()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !StepsOfServiceInstructions.activeInHierarchy)
        {
            stepsOfServiceReminder.enabled = false;
            StepsOfServiceInstructions.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Tab) && StepsOfServiceInstructions.activeInHierarchy)
        {
            stepsOfServiceReminder.enabled = true;
            StepsOfServiceInstructions.SetActive(false);
        }
    }

    public void HideUI()
    {
        timeText.enabled = false;
        totalSalesText.enabled = false;
        totalTipsText.enabled = false;
        totalTipsPercentText.enabled = false;
        stepsOfServiceReminder.enabled = false;
    }

    public void UnhideUI()
    {
        timeText.enabled = true;
        totalSalesText.enabled = true;
        totalTipsText.enabled = true;
        totalTipsPercentText.enabled = true;
        stepsOfServiceReminder.enabled = true;
    }

    /*public void ClearPlayerHand()
    {
    if(playerInteraction.lastTableTouched)
    {

    }
    playerInteraction.isCarryingPlate = false;
    playerInteraction.isCarryingDirtyPlate = false;
    playerInteraction.plateTableDestination = 0;
    playerInteraction.isCarryingCheck = false; 
    playerInteraction.checkInHand = null;
    playerInteraction.plateTouched = null;
    }*/

    public void DisplayLastTableTouched()
    {
        if(playerInteraction.GetLastTableTouched() != null)
        {
            lastTableTouchedText.text = "Last Table Touched: " + playerInteraction.GetLastTableTouched().name;
        }
        else
        {
            lastTableTouchedText.text = "Last Table Touched: ";
        }
    }

    public void SetSalesAndTipsText(float sales, float tips)
    {
        totalSalesText.text = "Total Sales: " + string.Format("{0:C}", sales);
        totalTipsText.text = "Total Tips: " + string.Format("{0:C}", tips);
        if(sales > 0)
        {
            float tipPercent = tips / sales;
            totalTipsPercentText.text = "Tip Percent: " + Mathf.Round(tipPercent * 10000) / 100 + "%";
        }
        else
        {
            totalTipsPercentText.text = "Tip Percent: 0%";
        }
    }
}
