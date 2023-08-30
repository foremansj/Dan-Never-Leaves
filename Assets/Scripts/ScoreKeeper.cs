using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public float totalSalesAmount;
    public float totalTipsAmount;
    public float totalTipsPercentAmount;

    UIController uIController;

    static ScoreKeeper instance;
    private void Awake()
    {
        uIController = FindObjectOfType<UIController>();
        ManageSingleton();
    }

    private void Start()
    {
        
        uIController.SetSalesAndTipsText(totalSalesAmount, totalTipsAmount);
    }

    // Update is called once per frame
    void ManageSingleton()
    {
        if(instance != null && instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void IncrementTotalSales(float num)
    {
        totalSalesAmount += num;
        uIController.totalSalesText.text = "Total Sales: " + string.Format("{0:C}", totalSalesAmount);
    }

    public void IncrementTotalTips(float num)
    {
        totalTipsAmount += num;
        uIController.totalTipsText.text = "Total Tips: " + string.Format("{0:C}", totalTipsAmount);
    }

    public void IncrementTipsPercent()
    {
        if(totalSalesAmount > 0)
        {
            totalTipsPercentAmount = totalTipsAmount / totalSalesAmount;
            uIController.totalTipsPercentText.text = "Tip Percent: " + Mathf.Round(totalTipsPercentAmount * 10000) / 100 + "%";
        }
        else
        {
            uIController.totalTipsPercentText.text = "Tip Percent: 0%";
        }
    }

    public void ResetScore()
    {
        totalSalesAmount = 0;
        totalTipsAmount = 0;
        totalTipsPercentAmount = 0;
        uIController.SetSalesAndTipsText(0,0);
    }

    public void CallScore()
    {
        uIController = FindObjectOfType<UIController>();
        uIController.totalSalesText.text = "Total Sales: " + string.Format("{0:C}", totalSalesAmount);
        uIController.totalTipsText.text = "Total Tips: " + string.Format("{0:C}", totalTipsAmount);
        if(totalSalesAmount > 0)
        {
            totalTipsPercentAmount = totalTipsAmount / totalSalesAmount;
            uIController.totalTipsPercentText.text = "Tip Percent: " + Mathf.Round(totalTipsPercentAmount * 10000) / 100 + "%";
        }
        else
        {
            uIController.totalTipsPercentText.text = "Tip Percent: 0%";
        }
    }
}
