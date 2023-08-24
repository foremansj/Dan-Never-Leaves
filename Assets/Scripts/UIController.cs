using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI totalSalesText;
    [SerializeField] TextMeshProUGUI totalTipsText;
    [SerializeField] TextMeshProUGUI totalTipsPercentText;
    
    public float totalSalesAmount;
    public float totalTipsAmount;
    public float totalTipsPercentAmount;

    [SerializeField] float clockSpeed = 2f;
    float seconds;
    float minutes;
    float hours = 5f;
    
    bool isPaused;

    private void Awake()
    {
        IncrementTotalSales(0f);
        IncrementTotalTips(0f);
    }

    private void Start()
    {
        IncrementTipsPercent();
    }

    // Update is called once per frame
    void Update()
    {
        PauseGame();
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
    }

    public void IncrementTotalSales(float num)
    {
        totalSalesAmount += num;
        totalSalesText.text = "Total Sales: " + string.Format("{0:C}", totalSalesAmount);
    }

    public void IncrementTotalTips(float num)
    {
        totalTipsAmount += num;
        totalTipsText.text = "Total Tips: " + string.Format("{0:C}", totalTipsAmount);
    }

    public void IncrementTipsPercent()
    {
        if(totalSalesAmount > 0)
        {
            totalTipsPercentAmount = totalTipsAmount / totalSalesAmount;
            totalTipsPercentText.text = "Tip Percent: " + Mathf.Round(totalTipsPercentAmount * 10000) / 100 + "%";
        }
        else
        {
            totalTipsPercentText.text = "Tip Percent: 0%";
        }
    }

    private void PauseGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            Time.timeScale = 0;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
        }

        else if(Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
