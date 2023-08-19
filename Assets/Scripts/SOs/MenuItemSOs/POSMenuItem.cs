using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class POSMenuItem : MonoBehaviour
{
    public MenuItemSO menuItemSO;
    [SerializeField] TextMeshProUGUI buttonText;

    public string itemName;
    public string itemDescription;

    public float itemPrice;
    public float itemTaxRate;
    public float totalItemCost;

    public float ticketTime;

    
    void Start()
    {
        buttonText.text = menuItemSO.itemName;
        
        itemName = menuItemSO.itemName;
        itemDescription = menuItemSO.description;

        itemPrice = menuItemSO.baseCost;
        itemTaxRate = menuItemSO.taxRate;
        totalItemCost = itemPrice + (itemPrice * itemTaxRate);

        ticketTime = menuItemSO.ticketTimeSeconds;

    }

    public MenuItemSO GetMenuItemSO()
    {
        return menuItemSO;
    }
}
