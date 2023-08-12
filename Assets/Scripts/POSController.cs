using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class POSController : MonoBehaviour
{
    //[Header("Receipt Text")]
    //[SerializeField] TextMeshProUGUI itemText;
    //[SerializeField] TextMeshProUGUI qtyText;
    //[SerializeField] TextMeshProUGUI eachCostText;
    //[SerializeField] TextMeshProUGUI totalCostText;

    [Header("POS Panels")]
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject receiptPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject floorMapPanel;

    [Header("Checks & Orders")]
    [SerializeField] GameObject checkPrefab;
    [SerializeField] GameObject orderPrefab;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemQTYText;
    [SerializeField] TextMeshProUGUI individualCostText;
    [SerializeField] TextMeshProUGUI totalItemCost;

    [Header("Order Totals Text")]
    [SerializeField] TextMeshProUGUI subtotalText;
    [SerializeField] TextMeshProUGUI taxTotalText;
    [SerializeField] TextMeshProUGUI balanceTotalText;
    [SerializeField] TextMeshProUGUI totalCheckCostText;
    [SerializeField] TextMeshProUGUI creditsTotalText;
    [SerializeField] TextMeshProUGUI discountsTotalText;
    [SerializeField] TextMeshProUGUI tipPercentText;
    [SerializeField] TextMeshProUGUI tipTotalText;

    
    int receiptNumberCounter = 1;

    PlayerInput playerInput;
    TableController tableController;
    CustomerCheck activeCheck;
    CustomerCheck originalCheck;
    

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Start()
    {
        tableController = GetComponent<TableController>();
    }
    
    
    public void OpenFloorMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
        backgroundPanel.SetActive(true);
        floorMapPanel.SetActive(true);
    }

    public void OpenMenuPanels()
    {
        floorMapPanel.SetActive(false);
        receiptPanel.SetActive(true);
        menuPanel.SetActive(true);
        SetOrderScreen(activeCheck);
    }

    public void EscapePOS()
    {
        floorMapPanel.SetActive(false);
        receiptPanel.SetActive(false);
        menuPanel.SetActive(false);
        backgroundPanel.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void CreateCustomerCheck(TableController tableController)
    {
        GameObject checkHolder = tableController.GetLinkedTable().transform.Find("Check Holder").gameObject;
        if(checkHolder.transform.childCount > 0)            // this.transform.Find("Check Holder").childCount > 0)
        {
            GameObject newCheck = Instantiate(checkHolder.transform.GetChild(0).gameObject, new Vector3(0,0,0), Quaternion.identity);
            newCheck.transform.SetParent(checkHolder.transform);
            newCheck.name = "Temp Check #" + newCheck.GetComponent<CustomerCheck>().checkNumber;
            Debug.Log("active check = " + activeCheck);
            
            activeCheck = newCheck.GetComponent<CustomerCheck>();
            
            return;
        }
        
        else
        {
            GameObject newCheck = Instantiate(checkPrefab, new Vector3(0,0,0), Quaternion.identity);
            newCheck.transform.SetParent(checkHolder.transform);
            newCheck.GetComponent<CustomerCheck>().checkNumber = receiptNumberCounter;
            newCheck.GetComponent<CustomerCheck>().tableNumber = tableController.GetTableNumber();
            newCheck.name = "Temp Check #" + newCheck.GetComponent<CustomerCheck>().checkNumber;
            

            activeCheck = newCheck.GetComponent<CustomerCheck>();
            receiptNumberCounter++;
            //activeCheck.checkNumber = receiptNumberCounter;
            //activeCheck.tableNumber = tableController.GetTableNumber();
            //originalCheck = activeCheck;
            
            //activeCheck.name = "Temp Check #" + receiptNumberCounter;
            
            
            SetOrderScreen(activeCheck);
            Debug.Log("Active Check = " + activeCheck);
        }
        
    }

    void SetOrderScreen(CustomerCheck check)
    {
        activeCheck = check;
        ResetCheckScreen();

        float subtotal = 0;
        float taxTotal = 0;


        check.CalculateTotals();

        for (int i = 0; i < check.itemNames.Count; i++)
        {
            //print item, then return to new line
            itemNameText.text += check.itemNames[i] + "<br>";
            //print qty, then return to new line
            itemQTYText.text += check.itemQuantities[i].ToString() + "<br>";
            //print individual item cost, then return to new line
            individualCostText.text += string.Format("{0:C}", check.itemBaseCosts[i]) + "<br>";
            //print total cost, then return to new line
            totalItemCost.text += string.Format("{0:C}", check.individualItemTotals[i]) + "<br>";

            //add items to bill total
            subtotal += check.itemQuantities[i] * check.itemBaseCosts[i];
            taxTotal += check.itemQuantities[i] * check.itemTaxAmounts[i];

            subtotalText.text = string.Format("{0:C}", subtotal);
            taxTotalText.text = string.Format("{0:C}", taxTotal);
            balanceTotalText.text = string.Format("{0:C}", subtotal + taxTotal);
            creditsTotalText.text = string.Format("{0:C}", check.creditsAmount);
            discountsTotalText.text = string.Format("{0:C}", check.discountsAmount);
            tipPercentText.text = $"Tip ({check.tipPercent * 100}%):";
            tipTotalText.text = string.Format("{0:C}", check.tipAmount);
            totalCheckCostText.text = string.Format("{0:C}", subtotal + taxTotal - check.creditsAmount - check.discountsAmount + check.tipAmount);
        }

    }

    void ResetCheckScreen()
    {
        itemNameText.text = null;
        itemQTYText.text = null;
        individualCostText.text = null;
        totalItemCost.text = null;
        subtotalText.text = string.Format("{0:C}", 0);
        taxTotalText.text = string.Format("{0:C}", 0);

        balanceTotalText.text = string.Format("{0:C}", 0);
        creditsTotalText.text = string.Format("{0:C}", 0);
        discountsTotalText.text = string.Format("{0:C}", 0);
        tipTotalText.text = string.Format("{0:C}", 0);
        totalCheckCostText.text = string.Format("{0:C}", 0);
    }

    public void AddItemToCheck(MenuItemSO item)
    {
        if(activeCheck.itemNames.Contains(item.itemName))
        {
            int index = activeCheck.itemNames.IndexOf(item.itemName);
            activeCheck.itemQuantities[index] += 1;
            activeCheck.CalculateTotals();
            SetOrderScreen(activeCheck);
        }

        else
        {
            activeCheck.itemNames.Add(item.itemName);
            activeCheck.itemQuantities.Add(1);
            activeCheck.itemTaxAmounts.Add(item.taxCost);
            activeCheck.itemBaseCosts.Add(item.baseCost);
            activeCheck.individualItemTotals.Add(item.GetTotalCost());
            activeCheck.CalculateTotals();
            SetOrderScreen(activeCheck);
        }
    }

    public void SendChanges()
    {
        //if the current active order is on a table that was already established, we need to replace the original check with the updated one
        //and we need to assign a new name to the temp check, and update the check number
        //then we need to delete the original (now outdated) check
        if(activeCheck.transform.parent.childCount > 1)
        {
            activeCheck.name = "Check #" + activeCheck.transform.parent.GetChild(0).GetComponent<CustomerCheck>().checkNumber;
            Destroy(activeCheck.transform.parent.GetChild(0).gameObject);
        }
        //otherise is only one child object, there is only one check, we need to save it and update the name
        else
        {
            activeCheck.name = "Check #" + activeCheck.checkNumber;
        }

    }

    public void CancelChanges()
    {
        //the current temporary check will be deleted without saving any changes
        Destroy(activeCheck.gameObject);
    }

    
    public int GetReceiptNumber()
    {
        return receiptNumberCounter;
    }
}
