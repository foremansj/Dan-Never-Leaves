using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;

public class POSController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject loginScreenPanel;
    [SerializeField] GameObject receiptPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject floorMapPanel;
    [SerializeField] GameObject paymentPanel;
    [SerializeField] GameObject openNotesButton;
    [SerializeField] GameObject closeNotesButton;
    [SerializeField] GameObject toastWithNotes;
    [SerializeField] GameObject notesWithToast;
    [SerializeField] GameObject mainToast;
    [SerializeField] GameObject mainNotes;

    [Header("Checks & Orders")]
    [SerializeField] GameObject checkPrefab;
    [SerializeField] GameObject closedCheckHolder;
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
    PlayerInteraction playerInteraction;
    
    POSTableController tableController;
    CameraController cameraController;
    
    public CustomerCheck activeCheck;
    int activeTableNumber;

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        cameraController = FindObjectOfType<CameraController>();
    }

    void Start()
    {
        tableController = GetComponent<POSTableController>();
    }
    
    
    public void OpenFloorMap()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
        backgroundPanel.SetActive(true);
        floorMapPanel.SetActive(true);
    }

    public void OpenMenuPanels()
    {
        floorMapPanel.SetActive(false);
        receiptPanel.SetActive(true);
        menuPanel.SetActive(true);
        SetOrderScreen(activeCheck);
        openNotesButton.SetActive(true);
    }

    public void EscapePOS()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
        floorMapPanel.SetActive(false);
        receiptPanel.SetActive(false);
        menuPanel.SetActive(false);
        backgroundPanel.SetActive(false);
        openNotesButton.SetActive(false);
        cameraController.SwitchCameras();
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void CreateCustomerCheck(POSTableController tableController)
    {
        GameObject checkHolder = tableController.GetLinkedTable().transform.Find("Check Holder").gameObject;
        if(checkHolder.transform.childCount > 0)            // this.transform.Find("Check Holder").childCount > 0)
        {
            GameObject newCheck = Instantiate(checkHolder.transform.GetChild(0).gameObject, new Vector3(0,0,0), Quaternion.identity);
            newCheck.transform.SetParent(checkHolder.transform);
            newCheck.name = "Temp Check #" + newCheck.GetComponent<CustomerCheck>().checkNumber;
            
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
            
            SetOrderScreen(activeCheck);
        }
    }

    void SetOrderScreen(CustomerCheck check)
    {
        activeCheck = check;
        activeTableNumber = activeCheck.GetTableNumber();
        ResetCheckScreen();
        NormalizeToastScreens(activeCheck);

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
        //if there is more than one check on the table, rename the new check, destroy the old check, clear the active check
        if(activeCheck.transform.parent.childCount > 1)
        {
            activeCheck.name = "Check #" + activeCheck.transform.parent.GetChild(0).GetComponent<CustomerCheck>().checkNumber;
            Destroy(activeCheck.transform.parent.GetChild(0).gameObject);
            activeCheck = null;
        }
        //if there is only one check, rename the check, then clear the active check
        else
        {
            activeCheck.name = "Check #" + activeCheck.checkNumber;
            activeCheck = null;
        }
    }

    public void SendAndStay()
    {
        //Same method as send and stay but without resetting the active check
        if(activeCheck.transform.parent.childCount > 1)
        {
            activeCheck.name = "Check #" + activeCheck.transform.parent.GetChild(0).GetComponent<CustomerCheck>().checkNumber;
            Destroy(activeCheck.transform.parent.GetChild(0).gameObject);
        }
        
        else
        {
            activeCheck.name = "Check #" + activeCheck.checkNumber;
        }
    }

    public void CancelChanges()
    {
        //the current temporary check will be deleted without saving any changes
        if(activeCheck.name.Contains("Temp"))
        {
            Destroy(activeCheck.gameObject);
        }
        
        EscapePOS();
    }

    public void CloseCheck()
    {
        activeCheck.transform.SetParent(closedCheckHolder.transform);
        activeCheck = null;
        //move check to closed check holder
    }
    
    public void OpenPaymentScreen()
    {

    }

    public int GetReceiptNumber()
    {
        return receiptNumberCounter;
    }

    public void NormalizeToastScreens(CustomerCheck check)
    {
        if(gameObject == mainToast) //&& toastWithNotes.activeInHierarchy == false)
        {
            toastWithNotes.GetComponent<POSController>().activeCheck = activeCheck;
        }
        
        else if(gameObject == toastWithNotes)
        {
            mainToast.GetComponent<POSController>().activeCheck = activeCheck;
        }
    }

    public void OpenNotesToast()
    {     
        NormalizeToastScreens(activeCheck);
        
        mainToast.SetActive(false);
        /*backgroundPanel.SetActive(false);
        loginScreenPanel.SetActive(false);
        receiptPanel.SetActive(false);
        menuPanel.SetActive(false);
        floorMapPanel.SetActive(false);
        //paymentPanel.SetActive(false);
        openNotesButton.SetActive(false);
        */
        //closeNotesButton.SetActive(true);
        toastWithNotes.SetActive(true);
        notesWithToast.SetActive(true);
    }

    public void SetPOSNotepad()
    {
        GameObject table = activeCheck.transform.parent.parent.gameObject;
        
        ServerNotes mainNotepad = mainNotes.GetComponent<ServerNotes>();
        ServerNotes toastNotepad = notesWithToast.GetComponent<ServerNotes>();
        toastNotepad.SetNotesTableNumber(table);
        if(mainNotepad.workingTableNotes.ContainsKey(table.name))
        {
            notesWithToast.GetComponent<ServerNotes>().serverNotesInputField.text = mainNotepad.workingTableNotes[table.name];
        }
        else
        {
            notesWithToast.GetComponent<ServerNotes>().serverNotesInputField.text = "";
            return;
        }
    }

    public void CloseNotesWithToast()
    {
        NormalizeToastScreens(activeCheck);
        
        //closeNotesButton.SetActive(false);
        toastWithNotes.SetActive(false);
        notesWithToast.SetActive(false);

        mainToast.SetActive(true);
        /*backgroundPanel.SetActive(true);
        //loginScreenPanel.SetActive(true);
        receiptPanel.SetActive(true);
        menuPanel.SetActive(true);
        //floorMapPanel.SetActive(true);
        //paymentPanel.SetActive(true);
        openNotesButton.SetActive(true);*/
    }

    private void OnEnable()
    {
        if(activeCheck != null)
        {
            SetOrderScreen(activeCheck);
        }
    }
}
