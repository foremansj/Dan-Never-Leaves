using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckController : MonoBehaviour
{
    public int tableNumber;
    public int checkNumber;

    public List<string> itemNames = new List<string>();
    public List<int> itemQuantities = new List<int>();
    public List<float> itemTaxRates = new List<float>();
    public List<float> itemBaseCosts = new List<float>();
    public List<float> individualItemTotals = new List<float>();

    public Dictionary<MenuItemSO, int> actualFullOrder = new Dictionary<MenuItemSO, int>();
    public Dictionary<MenuItemSO, int> playerEnteredOrder = new Dictionary<MenuItemSO, int>();
    public Dictionary<MenuItemSO, int> tempPlayerEnteredOrder = new Dictionary<MenuItemSO, int>();

    public float taxTotal;
    public float subtotal;
    public float checkTotal;
    
    public float creditsAmount;
    public float discountsAmount;
    public float tipAmount;
    public float tipPercent;

    KitchenWindowController kitchenWindowController;
    public PartyController partyController;
    
    private void Awake()
    {
        kitchenWindowController = FindObjectOfType<KitchenWindowController>();
    }

    private void Start()
    {
        partyController = GetComponent<PartyController>();
    }

    private void Update()
    {
        //from the POS controller, get the check # unless I want to set that from the POS side, which I actually think I do
    }

    public int GetTableNumber()
    {
        return tableNumber;
    }

    public void AddToOrderDictionary(Dictionary<MenuItemSO, int> dict, MenuItemSO item)
    {
        if(dict.ContainsKey(item))
        {
            dict.TryGetValue(item, out int itemCount);
            dict[item] = itemCount + 1;
        }
        else
        {
            dict.Add(item, 1);
        }
    }

    public Dictionary<MenuItemSO, int> GetPlayerEnteredOrder()
    {
        return playerEnteredOrder;
    }

    public void CompileFullPartyOrder(MenuItemSO item)
    {
        if(actualFullOrder.ContainsKey(item))
        {
            actualFullOrder.TryGetValue(item, out int itemCount);
            actualFullOrder[item] = itemCount + 1;
        }
        else
        {
            actualFullOrder.Add(item, 1);
        }
    }

    public void SendToKitchen(Dictionary<MenuItemSO, int> ticket)
    {
        float ticketTime = kitchenWindowController.SetTicketTime(ticket);
        StartCoroutine(kitchenWindowController.StartCookingTicket(ticket, ticketTime));
    }

    public void CalculateCheckTotal()
    {
        checkTotal = 0;
        foreach (KeyValuePair<MenuItemSO, int> pair in actualFullOrder)
        {
            checkTotal += pair.Key.GetTotalCost() * pair.Value;
        }
    }

    public void ListOutOrder()
    {
        foreach (KeyValuePair<MenuItemSO, int> pair in actualFullOrder)
        {
            itemNames.Add(pair.Key.itemName);
            itemQuantities.Add(pair.Value);
            itemTaxRates.Add(pair.Key.taxRate);
            itemBaseCosts.Add(pair.Key.baseCost);
            individualItemTotals.Add(pair.Key.GetTotalCost());
        }
    }
}
