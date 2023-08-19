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
    
    CameraController cameraController;
    CustomerDialogue customerDialogue;
    public TableController tableTouched; 

    bool isTouchingPOS = false;
    
    private void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
        customerDialogue = FindObjectOfType<CustomerDialogue>();
    }
    void Start()
    {
       playerInput = GetComponent<PlayerInput>();
       
    }

    void OnInteract(InputValue value)
    {
        if(value != null && tableTouched != null && tableTouched.hasCustomersSeated)
        {
            orderingCanvas.SetActive(true);
            serverNotes.OpenTableNotes(tableTouched);
            
            //start customer dialogue at Seat 1
            customerDialogue.currentCustomerIndex = 0;
            customerDialogue.GenerateOrderDialogue(tableTouched.GetOrderBySeatNumber(0));
            customerDialogue.StartTypewriterCoroutine(customerDialogue.GetOrderText());
            //StartCoroutine(customerDialogue.TypewriteOrder(customerDialogue.GetOrderText()));
            cameraController.SwitchCameras();
            //cameraController.HardLookAtObject(tableTouched.GetComponent<TableController>().GetCustomerAtSeat(0));
            playerInput.SwitchCurrentActionMap("Taking Orders");
        }

        if(value != null && isTouchingPOS == true)
        {
            playerInput.SwitchCurrentActionMap("UI");
            cameraController.SwitchCameras();
            cameraController.HardLookAtObject(pOSController.gameObject);
            pOSController.enabled = true;
            pOSController.OpenFloorMap();
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            tableTouched = other.GetComponent<TableController>();
            TableController tableControl = tableTouched.GetComponent<TableController>();
            cameraController.MoveHardLookCamera(tableTouched.transform);
            if(tableControl.hasCustomersSeated)
            {
                GameObject customerHead = tableControl.GetCurrentParty().partyCustomers[0].GetComponent<CustomerController>().GetCustomerHead();
                cameraController.HardLookAtObject(customerHead);
            }
        }

        if(other.transform.tag == "POS")
        {
            isTouchingPOS = true;
            cameraController.MoveHardLookCamera(gameObject.transform);
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            tableTouched = null;
        }
        
        if(other.transform.tag == "POS")
        {
            isTouchingPOS = false;
        }
    }

    public TableController GetTableTouched()
    {
        return tableTouched;
    }
}
