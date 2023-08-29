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
    [SerializeField] GameObject StepsOfServiceInstructions;
    
    public float totalSalesAmount;
    public float totalTipsAmount;
    public float totalTipsPercentAmount;

    [SerializeField] float clockSpeed = 2f;
    float seconds;
    float minutes;
    float hours = 5f;

    PlayerInteraction playerInteraction;

    private void Awake()
    {
        IncrementTotalSales(0f);
        IncrementTotalTips(0f);
        playerInteraction = FindObjectOfType<PlayerInteraction>();
    }

    private void Start()
    {
        IncrementTipsPercent();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    

    private void ViewStepsOfService()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !StepsOfServiceInstructions.activeInHierarchy)
        {
            StepsOfServiceInstructions.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Tab) && StepsOfServiceInstructions.activeInHierarchy)
        {
            StepsOfServiceInstructions.SetActive(false);
        }
    }

    public void HideUI()
    {
        timeText.enabled = false;
        totalSalesText.enabled = false;
        totalTipsText.enabled = false;
        totalTipsPercentText.enabled = false;
    }

    public void UnhideUI()
    {
        timeText.enabled = true;
        totalSalesText.enabled = true;
        totalTipsText.enabled = true;
        totalTipsPercentText.enabled = true;
    }

    public void ClearPlayerHand()
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
    }
}
