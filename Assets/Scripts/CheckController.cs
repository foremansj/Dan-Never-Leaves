using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckController : MonoBehaviour
{
    public int tableNumber;
    public int checkNumber = 0;

    public List<string> itemNames = new List<string>();
    public List<int> itemQuantities = new List<int>();
    public List<float> itemTaxRates = new List<float>();
    public List<float> itemBaseCosts = new List<float>();
    public List<float> individualItemTotals = new List<float>();

    public Dictionary<MenuItemSO, int> actualFullOrder = new Dictionary<MenuItemSO, int>();
    public Dictionary<MenuItemSO, int> playerEnteredOrder = new Dictionary<MenuItemSO, int>();
    public Dictionary<MenuItemSO, int> currentKitchenTicket = new Dictionary<MenuItemSO, int>();

    public float taxTotal;
    public float subtotal;
    public float checkTotal;
    
    public float creditsAmount;
    public float discountsAmount;
    public float tipAmount;
    public float tipPercent;
    public bool isReadyToClose = false;
    public bool justNeedsATip = false;

    KitchenWindowController kitchenWindowController;
    UIController uIController;
    ScoreKeeper scoreKeeper;
    public PartyController partyController;
    
    private void Awake()
    {
        kitchenWindowController = FindObjectOfType<KitchenWindowController>();
        uIController = FindObjectOfType<UIController>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
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

    public void SendToKitchen()
    {
        float ticketTime = kitchenWindowController.SetTicketTime(currentKitchenTicket);
        StartCoroutine(kitchenWindowController.StartCookingTicket(tableNumber, currentKitchenTicket, ticketTime));
    }

    public void CalculateCheckTotals()
    {
        foreach (KeyValuePair<MenuItemSO, int> pair in playerEnteredOrder)
        {
            taxTotal += pair.Key.taxRate * pair.Key.baseCost * pair.Value;
            subtotal += pair.Key.GetTotalCost() * pair.Value;
        }
        checkTotal = subtotal + taxTotal + tipAmount;
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

    public void SetCheckNumber(int number)
    {
        checkNumber = number;
    }

    public void CloseCheck()
    {
        if(isReadyToClose)
        {
            scoreKeeper.IncrementTotalSales(subtotal);
            scoreKeeper.IncrementTipsPercent();
            justNeedsATip = true;
        }
    }

    public void SignReceiptTipAndLeave()
    {
        for(int i = 0; i < 3; i++)
        {
            float randomTip = Random.Range(.15f, .25f);
            if(randomTip > tipPercent)
            {
                tipPercent = randomTip;
            }
        }
        tipAmount = subtotal * tipPercent;
        scoreKeeper.IncrementTotalTips(tipAmount);
        scoreKeeper.IncrementTipsPercent();
        //move check to database or simply add numbers to UI counter
        //maybe saving checks in a database is a later thing
        partyController.LeaveRestaurant();
        Destroy(this, 5f);
        //put table back on host's open table list
        //delete/clear table notes
    }
}
