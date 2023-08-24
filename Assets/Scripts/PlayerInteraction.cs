using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;
using System;

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
    TableController tableTouched; 
    public TableController lastTableTouched;
    public GameObject plateTouched;

    bool isCarryingPlate = false;
    bool isCarryingDirtyPlate = false;
    int plateTableDestination;
    public bool isCarryingCheck = false; 
    bool isInteractPressed;
    bool isBusy;
    
    private void Awake()
    {
        kitchenWindowController = FindObjectOfType<KitchenWindowController>();
        cameraController = FindObjectOfType<CameraController>();
        customerDialogue = FindObjectOfType<CustomerDialogue>();
    }
    private void Start()
    {
       playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        CheckIfInteractable();
    }

    void OnInteract(InputValue value)
    {
        isInteractPressed = value.isPressed;
    }

    private void InteractWithPOS()
    {
        playerInput.SwitchCurrentActionMap("UI");
        cameraController.SwitchCameras();
        cameraController.HardLookAtObject(pOSController.gameObject);
        pOSController.enabled = true;
        pOSController.OpenFloorMap();
    }

    private void TakeCustomerOrder()
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

    void CheckIfInteractable()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = ~LayerMask.GetMask("Player", "Ignore Raycast"); //ray does not touch player or ignore ray objects
        
        if (Physics.Raycast(ray, out hit, 2.5f, mask)) {    
            Transform objectHit = hit.transform;
            
            //check if can interact with tables
            if(objectHit.GetComponent<TableController>() != null && objectHit.GetComponent<TableController>().GetCurrentParty() != null)
            {
                tableTouched = objectHit.GetComponent<TableController>();
                
                if(!isCarryingPlate && !isCarryingCheck && tableTouched.hasCustomersSeated && !tableTouched.isFoodDropped && !isBusy)
                {//check if player can take a table's order
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        TakeCustomerOrder();
                        lastTableTouched = tableTouched;
                    }
                }

                else if(isCarryingPlate && !isCarryingCheck && tableTouched.GetTableNumber() == plateTableDestination && !isBusy)
                {//check if player can drop food at a table
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        PlaceFoodAtTable();
                    }
                }

                else if(!isCarryingPlate && !tableTouched.GetIsTableStillEating() && tableTouched.isFoodDropped && !isCarryingCheck && !isBusy)
                {//check if player can clear plates from a table
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        PickUpDirtyPlates();
                    }
                }

                else if(isCarryingCheck && tableTouched.currentParty.isReadyToPay && !isBusy)
                {//check if player can drop a check at a table
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        StartCoroutine(DropOffCheck(hit));
                    }
                }

                else
                {
                    return;
                }
            }
            //check if can interact with POS
            else if(objectHit.tag == "POS" && !isCarryingCheck && !isBusy)
            {//check if player can interact with the POS
                cameraController.ChangeReticleColor(Color.green);
                cameraController.MoveHardLookCamera(gameObject.transform);
                if(isInteractPressed)
                {
                    InteractWithPOS();
                }
            }
            //check if can interact with plate on kitchen window
            else if(hit.collider.tag == "Plates" && !isCarryingPlate && !isCarryingCheck && !isBusy)
            {
                plateTouched = hit.collider.transform.gameObject;
                cameraController.ChangeReticleColor(Color.green);
                if(isInteractPressed)
                {
                    PickUpFood(plateTouched);
                }
            }
            //check if can put dirty dish in dish pit
            else if(objectHit.tag == "Dish Pit" && isCarryingDirtyPlate && !isCarryingCheck && !isBusy)
            {
                cameraController.ChangeReticleColor(Color.green);
                if(isInteractPressed)
                {
                    PutDishesInWash();
                }
            }

            else
            {//if the player is not looking at an interactable, clear necessary variables
                tableTouched = null;
                plateTouched = null;
                cameraController.ChangeReticleColor(Color.red);
                return;
            }
        }
        else
        {//if the player is not looking at an interactable, clear necessary variables
            tableTouched = null;
            plateTouched = null;
            cameraController.ChangeReticleColor(Color.red);
            return;
        }
    }

    public TableController GetLastTableTouched()
    {
        return lastTableTouched;
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
        for(int i = 0; i < tableTouched.GetCurrentParty().partyCustomers.Count; i++)
        {
            if(tableTouched.GetCurrentParty().partyCustomers[i].GetComponent<CustomerController>().GetIsEating())
            {
                return;
            }
        }
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

    IEnumerator DropOffCheck(RaycastHit hit)
    {
        isCarryingCheck = false;
        GameObject checkPresenter = gameObject.transform.Find("Arm With Check").Find("Check Presenter Prefab Open(Clone)").gameObject;
        checkPresenter.transform.parent = null;
        
        while(checkPresenter.transform.position != hit.point)
        {
            checkPresenter.transform.position = Vector3.MoveTowards(checkPresenter.transform.position, hit.point, 0.01f * Time.deltaTime);
        }
        Vector3 rotation = checkPresenter.transform.eulerAngles;
        checkPresenter.transform.eulerAngles = new Vector3(-90, rotation.y, rotation.z);
        checkPresenter.transform.parent = hit.transform;
        checkPresenter.name = "Check Presenter";
        
        gameObject.transform.Find("Arm With Check").transform.gameObject.SetActive(false);
        isBusy = true;
        
        StartCoroutine(hit.transform.GetComponent<TableController>().currentParty.PayCheck());
        yield return new WaitForSeconds(2f);

        isBusy = false;
    }
}
