using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;

public class PlayerInteraction : MonoBehaviour
{
    public PlayerInput playerInput;

    [SerializeField] GameObject orderingCanvas;
    [SerializeField] ServerNotes serverNotes;
    [SerializeField] POSController pOSController;
    [SerializeField] GameObject armWithPlate;
    public GameObject plateCarried;
    
    CameraController cameraController;
    CustomerDialogue customerDialogue;
    KitchenWindowController kitchenWindowController;
    public TableController tableTouched; 
    public GameObject plateTouched;

    bool isTouchingPOS = false;
    bool isCarryingPlate = false;
    bool isCarryingDirtyPlate = false;
    bool isTouchingDishPit = false;
    int plateTableDestination;
    
    private void Awake()
    {
        kitchenWindowController = FindObjectOfType<KitchenWindowController>();
        cameraController = FindObjectOfType<CameraController>();
        customerDialogue = FindObjectOfType<CustomerDialogue>();
    }
    void Start()
    {
       playerInput = GetComponent<PlayerInput>();
       
    }

    void OnInteract(InputValue value)
    {
        //get customer order and take notes
        if(value != null && tableTouched != null && tableTouched.hasCustomersSeated && !tableTouched.foodDropped &&!isCarryingPlate)
        {
            orderingCanvas.SetActive(true);
            serverNotes.OpenTableNotes(tableTouched);

            cameraController.MoveHardLookCamera(tableTouched.transform);
            GameObject customerHead = tableTouched.GetCurrentParty().partyCustomers[0].GetComponent<CustomerController>().GetCustomerHead();
            cameraController.HardLookAtObject(customerHead);
            
            //start customer dialogue at Seat 1
            customerDialogue.currentCustomerIndex = 0;
            customerDialogue.GenerateOrderDialogue(tableTouched.GetOrderBySeatNumber(0));
            customerDialogue.StartTypewriterCoroutine(customerDialogue.GetOrderText());
            cameraController.SwitchCameras();
            playerInput.SwitchCurrentActionMap("Taking Orders");
        }
        //interact with POS to place orders
        else if(value != null && isTouchingPOS == true)
        {
            playerInput.SwitchCurrentActionMap("UI");
            cameraController.SwitchCameras();
            cameraController.HardLookAtObject(pOSController.gameObject);
            pOSController.enabled = true;
            pOSController.OpenFloorMap();
        }
        //pick up plate from kitchen window
        else if(value != null && plateTouched != null && !isCarryingPlate)
        {
            PickUpFood(plateTouched);
        }
        //place food at table that ordered
        else if(value != null && isCarryingPlate && tableTouched != null && tableTouched.GetTableNumber() == plateTableDestination)
        {
            PlaceFoodAtTable();
        }
        //pick up plates from tables that are done eating
        else if(value != null && !isCarryingPlate && tableTouched != null && !tableTouched.GetIsTableStillEating() && tableTouched.foodDropped)
        {
            PickUpDirtyPlates();
        }

        else if(value != null && isCarryingDirtyPlate && isTouchingDishPit)
        {
            PutDishesInWash();
        }
        
        else
        {
            return;
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            tableTouched = other.GetComponent<TableController>();
        }

        else if(other.transform.tag == "POS")
        {
            isTouchingPOS = true;
            cameraController.MoveHardLookCamera(gameObject.transform);
        }
        
        else if(other.transform.tag == "Kitchen Window")
        {
            Debug.Log("Child count =" + other.transform.childCount);
            if(other.transform.childCount > 0)
            {
                plateTouched = other.transform.GetChild(0).gameObject;
            }
        }
        else if(other.transform.tag == "Dish Pit")
        {
            isTouchingDishPit = true;
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            tableTouched = null;
        }
        
        else if(other.transform.tag == "POS")
        {
            isTouchingPOS = false;
        }

        else if(other.transform.tag == "Kitchen Window")
        {
            plateTouched = null;
        }

        else if(other.transform.tag == "Dish Pit")
        {
            isTouchingDishPit = false;
        }
    }

    public TableController GetTableTouched()
    {
        return tableTouched;
    }

    void PickUpFood(GameObject obj)
    {
        armWithPlate.SetActive(true);
        armWithPlate.GetComponentInChildren<TextMeshProUGUI>().text = obj.GetComponentInChildren<TextMeshProUGUI>().text;
        kitchenWindowController.ReopenWindowSlot(plateTouched.transform.parent.gameObject);
        isCarryingPlate = true;
        plateTableDestination = int.Parse(plateTouched.name);
        plateTouched = null;
        //plateCarried = plateTouched;
        //plateCarried.transform.position = armWithPlate.transform.GetChild(0).position;
        //plateCarried.transform.SetParent(armWithPlate.transform);
    }

    void PlaceFoodAtTable()
    {
        tableTouched.PlaceFoodForCustomers();
        armWithPlate.SetActive(false);
        plateTableDestination = 0;
        isCarryingPlate = false;
    }

    void PickUpDirtyPlates()
    {
        armWithPlate.SetActive(true);
        armWithPlate.GetComponentInChildren<TextMeshProUGUI>().text = "Dirty";
        isCarryingDirtyPlate = true;
        tableTouched.RemovePlatesFromTable();
        tableTouched.currentParty.isReadyToPay = true;
    }

    void PutDishesInWash()
    {
        armWithPlate.SetActive(false);
        isCarryingDirtyPlate = false;
    }
}
