using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using System;

public class POSController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] GameObject backgroundPanel;
    //[SerializeField] GameObject loginScreenPanel;
    [SerializeField] GameObject closePOSButton;
    [SerializeField] GameObject backToFloormapButton;
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
    [SerializeField] GameObject closedCheckHolder;
    [SerializeField] GameObject checkPresenterPrefab;
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

    [SerializeField] GameObject player;

    int checkNumberCounter = 1;

    PlayerInput playerInput;
    
    CameraController cameraController;
    UIController uIController;
    
    public TableController tableController;
    public CheckController activeCheck;

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        cameraController = FindObjectOfType<CameraController>();
        uIController = FindObjectOfType<UIController>();
    }
    
    public void OpenFloorMap()
    {
        uIController.HideUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
        backgroundPanel.SetActive(true);
        closePOSButton.SetActive(true);
        backToFloormapButton.SetActive(false);
        floorMapPanel.SetActive(true);

        receiptPanel.SetActive(false);
        menuPanel.SetActive(false);
        openNotesButton.SetActive(false);

        for(int i = 0; i < floorMapPanel.transform.childCount; i++)
        {
            if(floorMapPanel.transform.GetChild(i).name.Contains("Table"))
            {
                TableController table = floorMapPanel.transform.GetChild(i).GetComponent<POSTableController>()
                                                            .GetLinkedTable().GetComponent<TableController>();
                if(table.GetCurrentParty() != null)
                {
                    floorMapPanel.transform.GetChild(i).GetComponent<Image>().color = Color.green;
                }
                else
                {
                    floorMapPanel.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                }
            }
            
            /*if(floorMapPanel.transform.GetChild(i).GetComponent<POSTableController>().GetLinkedTable().GetComponent<TableController>().GetCurrentParty() != null
                    && floorMapPanel.transform.GetChild(i).name.Contains("Table"))
            {
                floorMapPanel.transform.GetChild(i).GetComponent<Image>().color = Color.green;
            }
            else if(floorMapPanel.transform.GetChild(i).name.Contains("Image"))
            {
                //do nothing
            }
            else
            {
                floorMapPanel.transform.GetChild(i).GetComponent<Image>().color = Color.magenta;
            }*/
        }
    }

    public void OpenMenuPanels()
    {
        if(tableController.GetCurrentParty() != null)
        {
            floorMapPanel.SetActive(false);
            receiptPanel.SetActive(true);
            menuPanel.SetActive(true);
            backToFloormapButton.SetActive(true);
            SetOrderScreen();
            openNotesButton.SetActive(true);
            closePOSButton.SetActive(true);
        }
        else
        {
            return;
        }
    }

    public void EscapePOS()
    {
        uIController.UnhideUI();
        floorMapPanel.SetActive(false);
        receiptPanel.SetActive(false);
        menuPanel.SetActive(false);
        backgroundPanel.SetActive(false);
        openNotesButton.SetActive(false);
        closePOSButton.SetActive(false);
        backToFloormapButton.SetActive(false);
        cameraController.SwitchCameras();
        playerInput.SwitchCurrentActionMap("Player");
    }

    private void OnEnable()
    {
        if(activeCheck != null)
        {
            SetOrderScreen();
        }
    }
    public void OpenNotesToast()
    {     
        //NormalizeToastScreens(activeCheck);
        mainToast.SetActive(false);
        toastWithNotes.SetActive(true);
        notesWithToast.SetActive(true);
    }

    public void SetPOSNotepad()
    {
        int table = activeCheck.GetTableNumber();
        
        ServerNotes mainNotepad = mainNotes.GetComponent<ServerNotes>();
        ServerNotes toastNotepad = notesWithToast.GetComponent<ServerNotes>();
        toastNotepad.notesTableHeaderText.text = "Table #" + table;
        if(mainNotepad.workingTableNotes.ContainsKey(table))
        {
            notesWithToast.GetComponent<ServerNotes>().serverNotesInputField.text = mainNotepad.workingTableNotes[table];
        }
        else
        {
            notesWithToast.GetComponent<ServerNotes>().serverNotesInputField.text = "";
            return;
        }
    }

    public void CloseNotesWithToast()
    {
        //NormalizeToastScreens(activeCheck);
        
        toastWithNotes.SetActive(false);
        notesWithToast.SetActive(false);

        mainToast.SetActive(true);
    }

    public void SetActiveTable(TableController table)
    {
        tableController = table;
        if(table.GetCurrentParty() != null) //.GetComponent<CheckController>()
        {
            activeCheck = table.GetCurrentParty().GetComponent<CheckController>();
        }
        else
        {
            return;
        }
    }

    public void AddItemToOrderWindow(MenuItemSO item)
    {
        //adds item to ticket and player entererd order
        activeCheck.AddToOrderDictionary(activeCheck.currentKitchenTicket, item);
        activeCheck.AddToOrderDictionary(activeCheck.playerEnteredOrder, item);
        SetOrderScreen();
    }
    
    public void SetOrderScreen()
    {
        ResetCheckScreen();
        float subtotal = 0;
        float taxTotal = 0;
        
        //if there is an active check, set the screen, if not, set the null screen
        if(activeCheck.playerEnteredOrder != null)
        {
            foreach(KeyValuePair<MenuItemSO, int> pair in activeCheck.playerEnteredOrder)
            {
                //setting the top of the check/order POS screen
                itemNameText.text += pair.Key.itemName + "<br>";
                itemQTYText.text += pair.Value  + "<br>";      
                individualCostText.text += string.Format("{0:C}", pair.Key.baseCost + "<br>");       
                totalItemCost.text += string.Format("{0:C}", (pair.Key.baseCost * pair.Value) + "<br>");

                //setting the bottom of the check/order POS screen
                subtotal += pair.Key.baseCost * pair.Value;
                taxTotal += (pair.Key.baseCost * pair.Value) * pair.Key.taxRate;

                subtotalText.text = string.Format("{0:C}", subtotal);
                taxTotalText.text = string.Format("{0:C}", taxTotal);
                balanceTotalText.text = string.Format("{0:C}", subtotal + taxTotal);
                creditsTotalText.text = string.Format("{0:C}", activeCheck.creditsAmount);
                discountsTotalText.text = string.Format("{0:C}", activeCheck.discountsAmount);
                tipPercentText.text = $"Tip ({activeCheck.tipPercent * 100}%):";
                tipTotalText.text = string.Format("{0:C}", activeCheck.tipAmount);
                totalCheckCostText.text = string.Format("{0:C}", subtotal + taxTotal - activeCheck.creditsAmount - 
                                                                activeCheck.discountsAmount + activeCheck.tipAmount);   
            }
        }

        else
        {
            ResetCheckScreen();
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

    public void SendChanges()
    {
        //SwapDictionaries(activeCheck.playerEnteredOrder, activeCheck.currentKitchenTicket);
        //AddToPlayerEnteredOrderDict();
        if(activeCheck.currentKitchenTicket.Count > 0)
        {
            activeCheck.SendToKitchen();
            activeCheck.partyController.assignedTable.isReadyToEat = true;
            if(activeCheck.checkNumber == 0)
            {
                activeCheck.SetCheckNumber(checkNumberCounter);
                checkNumberCounter++;
            }
            activeCheck.currentKitchenTicket.Clear();
        }
        else
        {
            return;
        }

    }

    public void CancelChanges()
    {
        RemoveChangesFromOrderScreen();
        activeCheck.currentKitchenTicket.Clear();
        //SwapDictionaries(activeCheck.currentKitchenTicket, activeCheck.playerEnteredOrder);
    }

    public void RemoveChangesFromOrderScreen()
    {
        foreach(KeyValuePair<MenuItemSO, int> pair in activeCheck.currentKitchenTicket)
        {
            int oldValue;
            activeCheck.playerEnteredOrder.TryGetValue(pair.Key, out oldValue);
            activeCheck.playerEnteredOrder[pair.Key] = oldValue - pair.Value;
            if(activeCheck.playerEnteredOrder[pair.Key] <= 0)
            {
                activeCheck.playerEnteredOrder.Remove(pair.Key);
            }
        }
    }

    public void NormalizeToastScreens()
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

    public void PrintCustomerCheck() //print customer check and hopefully write the correct bill on it based on what player entered in POS
    {
        if(activeCheck.playerEnteredOrder != null)
        {
            activeCheck.CalculateCheckTotals();
            GameObject playerCheckArm = player.GetComponent<PlayerInteraction>().GetCheckArmTransform();
            playerCheckArm.SetActive(true);
            GameObject newCheck = Instantiate(checkPresenterPrefab, Vector3.zero, Quaternion.identity);
            //GameObject checkLocation = checkArm.transform.GetChild(0).gameObject;
            newCheck.transform.parent = playerCheckArm.transform;
            newCheck.transform.position = playerCheckArm.transform.GetChild(0).gameObject.transform.position;
            newCheck.transform.rotation = playerCheckArm.transform.GetChild(0).gameObject.transform.rotation;
            newCheck.transform.localScale = playerCheckArm.transform.GetChild(0).gameObject.transform.localScale;
            TextMeshProUGUI checkText = newCheck.GetComponentInChildren<TextMeshProUGUI>();
            
            //checkText.alignment = TextAlignmentOptions.Center;
            //checkText.alignment = TextAlignmentOptions.Top;
            checkText.text = "Table #" + activeCheck.tableNumber + "<br>";
            //<align= "center">"Table #" + activeCheck.tableNumber + "<br>";
            checkText.text += "Check #" + activeCheck.checkNumber + "<br>";

            foreach(KeyValuePair<MenuItemSO, int> pair in activeCheck.playerEnteredOrder)
            {
                checkText.text += pair.Key.itemName + string.Format("{0:C}", pair.Key.baseCost * pair.Value) + "<br>"; 
            }

            checkText.text += "Subtotal: " + string.Format("{0:C}", activeCheck.subtotal) + "<br>";
            checkText.text += "Taxes: " + string.Format("{0:C}", activeCheck.taxTotal) + "<br>";
            checkText.text += "Tip: " + "<br>";
            checkText.text += "Total: " + string.Format("{0:C}", activeCheck.subtotal + activeCheck.taxTotal);
            player.GetComponent<PlayerInteraction>().isCarryingCheck = true;
            player.GetComponent<PlayerInteraction>().checkInHand = activeCheck;
        }
        
    }

    public void PayCustomerCheck()
    {
        activeCheck.CloseCheck();
        activeCheck.partyController.assignedTable.isReadyToTipAndLeave = true;
    }
}
