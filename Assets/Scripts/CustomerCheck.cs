using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerCheck : MonoBehaviour
{
    public int tableNumber;
    public int checkNumber;

    public List<string> itemNames;
    public List<int> itemQuantities;
    public List<float> itemTaxAmounts;
    public List<float> itemBaseCosts;
    public List<float> individualItemTotals;

    public float taxTotal;
    public float subtotal;
    public float checkTotal;
    
    public float creditsAmount;
    public float discountsAmount;
    public float tipAmount;
    public float tipPercent;

    public void CalculateTotals()
    {
        taxTotal = 0;
        subtotal = 0;
        checkTotal = 0;

        for(int i = 0; i < itemNames.Count; i++)
        {
            taxTotal += itemQuantities[i] * itemTaxAmounts[i];
            subtotal += itemQuantities[i] * itemBaseCosts[i];
            individualItemTotals[i] = itemQuantities[i] * (itemTaxAmounts[i] + itemBaseCosts[i]);
            checkTotal += individualItemTotals[i];
        }

        tipAmount = tipPercent * checkTotal;
    }

    public int GetTableNumber()
    {
        return tableNumber;
    }
}
