using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public PlayerInput playerInput;

    [SerializeField] GameObject orderingCanvas;
    [SerializeField] ServerNotes serverNotes;
    [SerializeField] POSController pOSController;
    [SerializeField] GameObject armWithPlate;
    [SerializeField] GameObject armForCheckPresenters;
    public GameObject plateCarried;
    
    CameraController cameraController;
    CustomerDialogue customerDialogue;
    KitchenWindowController kitchenWindowController;
    public TableController tableTouched; 
    public TableController lastTableTouched;
    public CheckController checkInHand;
    public GameObject plateTouched;

    bool isCarryingPlate = false;
    bool isCarryingDirtyPlate = false;
    public int plateTableDestination;
    public bool isCarryingCheck = false; 
    bool isInteractPressed;
    public bool isBusy;
    
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

    private void OpenSpecificOrderPOS(TableController table)
    {
        playerInput.SwitchCurrentActionMap("UI");
        cameraController.SwitchCameras();
        cameraController.HardLookAtObject(pOSController.gameObject);
        pOSController.enabled = true;
        pOSController.SetActiveTable(table);
        pOSController.OpenMenuPanels();
    }

    private void TakeCustomerOrder()
    {
        orderingCanvas.SetActive(true);
        serverNotes.OpenTableNotes(tableTouched);

        cameraController.MoveHardLookCamera(tableTouched.transform);
        CustomerController customer = tableTouched.GetCurrentParty().partyCustomers[0].GetComponent<CustomerController>();
        customer.SetHasOrdered();
        cameraController.HardLookAtObject(customer.GetCustomerHead());

        //start customer dialogue at Seat 1
        customerDialogue.currentCustomerIndex = 0;
        customerDialogue.GenerateOrderDialogue(tableTouched.GetOrderBySeatNumber(0));
        customerDialogue.StartTypewriterCoroutine(customerDialogue.GetOrderText());
        cameraController.SwitchCameras();
        playerInput.SwitchCurrentActionMap("Taking Orders");
        tableTouched.isReadyToOrder = false;
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
                
                if(tableTouched.isReadyToOrder && !isCarryingCheck && !isCarryingPlate && !isCarryingDirtyPlate)
                {//check if player can take a table's order
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        lastTableTouched = tableTouched;
                        cameraController.ChangeReticleColor(Color.red);
                        TakeCustomerOrder();
                    }
                }

                else if(tableTouched.isReadyToEat && isCarryingPlate && tableTouched.name == plateTableDestination.ToString())
                //isCarryingPlate && !isCarryingCheck && tableTouched.GetTableNumber() == plateTableDestination && !isBusy)
                {//check if player can drop food at a table
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        PlaceFoodAtTable();
                        cameraController.ChangeReticleColor(Color.red);
                    }
                }

                else if(tableTouched.isReadyToBus && !isCarryingCheck && !isCarryingPlate && !isCarryingDirtyPlate)
                {//check if player can clear plates from a table
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        PickUpDirtyPlates();
                        cameraController.ChangeReticleColor(Color.red);
                    }
                }

                else if(isCarryingCheck && tableTouched.isReadyForCheck && checkInHand.tableNumber.ToString() == tableTouched.name)
                {//check if player can drop a check at a table
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        tableTouched.isReadyForCheck = false;
                        StartCoroutine(DropOffCheck(hit));
                        cameraController.ChangeReticleColor(Color.red);
                    }
                }

                else if(!isCarryingCheck && !isCarryingPlate && !isCarryingDirtyPlate && tableTouched.GetHasDroppedCreditCard())
                {//check if check on table is ready for player to pick up and process payment
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        tableTouched.hasDroppedCreditCard = false;
                        PickUpCheckWithCard();
                        cameraController.ChangeReticleColor(Color.red);
                    }
                }

                else if(isCarryingCheck && checkInHand.tableNumber.ToString() == tableTouched.name && 
                                                                    tableTouched.isReadyToTipAndLeave)
                {//dropping off check last time)
                    cameraController.ChangeReticleColor(Color.blue);
                    if(isInteractPressed)
                    {
                        tableTouched.isReadyToTipAndLeave = false;
                        FinalCheckDrop();
                        cameraController.ChangeReticleColor(Color.red);
                    }
                }

                else
                {
                    return;
                }
            }
            //check if can interact with POS
            else if(objectHit.tag == "POS")
            {//check if player can interact with the POS
                cameraController.MoveHardLookCamera(gameObject.transform);
                if(!isCarryingCheck)
                {
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        InteractWithPOS();
                        cameraController.ChangeReticleColor(Color.red);
                    }
                }
                else if(isCarryingCheck && checkInHand != null)
                {
                    cameraController.ChangeReticleColor(Color.green);
                    if(isInteractPressed)
                    {
                        TableController table = checkInHand.partyController.assignedTable;
                        OpenSpecificOrderPOS(table);
                        cameraController.ChangeReticleColor(Color.red);
                    }

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
                    cameraController.ChangeReticleColor(Color.red);
                }
            }
            //check if can put dirty dish in dish pit
            else if(objectHit.tag == "Dish Pit" && isCarryingDirtyPlate)
            {
                cameraController.ChangeReticleColor(Color.green);
                if(isInteractPressed)
                {
                    PutDishesInWash();
                    cameraController.ChangeReticleColor(Color.red);
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
        plateCarried = plateTouched;
        armWithPlate.SetActive(true);
        armWithPlate.GetComponentInChildren<TextMeshProUGUI>().text = obj.GetComponentInChildren<TextMeshProUGUI>().text;
        kitchenWindowController.ReopenWindowSlot(plateTouched.transform.parent.gameObject);
        isCarryingPlate = true;
        plateTableDestination = int.Parse(plateTouched.name);
        plateTouched = null;
    }

    void PlaceFoodAtTable()
    {
        tableTouched.PlaceFoodForCustomers();
        armWithPlate.SetActive(false);
        plateTableDestination = 0;
        isCarryingPlate = false;
        tableTouched.isReadyToEat = false;
    }

    void PickUpDirtyPlates()
    {
        armWithPlate.SetActive(true);
        armWithPlate.GetComponentInChildren<TextMeshProUGUI>().text = "Dirty";
        isCarryingDirtyPlate = true;
        tableTouched.RemovePlatesFromTable();
        tableTouched.isReadyToBus = false;
        tableTouched.isReadyForCheck = true;
        //tableTouched.currentParty.isReadyForCheck = true;
    }

    void PutDishesInWash()
    {
        armWithPlate.SetActive(false);
        isCarryingDirtyPlate = false;
    }

    IEnumerator DropOffCheck(RaycastHit hit)
    {
        isCarryingCheck = false;
        GameObject checkPresenter = armForCheckPresenters.transform.Find("Check Presenter Prefab Open(Clone)").gameObject;
        checkPresenter.transform.parent = null;
        
        while(checkPresenter.transform.position != hit.point)
        {
            checkPresenter.transform.position = Vector3.MoveTowards(checkPresenter.transform.position, hit.point, 0.01f * Time.deltaTime);
        }
        Vector3 rotation = checkPresenter.transform.eulerAngles;
        checkPresenter.transform.eulerAngles = new Vector3(-90, rotation.y, rotation.z);
        checkPresenter.transform.parent = hit.transform;
        checkPresenter.name = "Check Presenter";
        
        armForCheckPresenters.transform.gameObject.SetActive(false);
        isBusy = true;
        
        float randomPaymentDelay = Random.Range(2500f, 3500f) * Time.deltaTime;
        StartCoroutine(hit.transform.GetComponent<TableController>().CloseCheckPresenter(randomPaymentDelay));
        yield return new WaitForSeconds(2f);

        isBusy = false;
    }

    public void PickUpCheckWithCard()
    {
        armForCheckPresenters.transform.gameObject.SetActive(true);
        lastTableTouched = tableTouched;
        checkInHand = lastTableTouched.GetCurrentParty().transform.GetComponent<CheckController>();
        GameObject checkPresenter = tableTouched.transform.Find("Check Presenter with Card").gameObject;
        checkPresenter.transform.SetParent(armForCheckPresenters.transform);
        GameObject checkSpot = armForCheckPresenters.transform.Find("CheckSpot").gameObject;
        //checkPresenter.transform.position = checkSpot.transform.position;
        checkPresenter.transform.localPosition = new Vector3(0.85f, 0.74f, -1.5f);
        checkPresenter.transform.localScale = checkSpot.transform.localScale;
        checkPresenter.transform.localEulerAngles = checkSpot.transform.localEulerAngles;

        isCarryingCheck = true;
        lastTableTouched.GetCurrentParty().GetComponent<CheckController>().isReadyToClose = true;
    }

    public GameObject GetCheckArmTransform()
    {
        return armForCheckPresenters;
    }

    public void FinalCheckDrop()
    {
        StartCoroutine(checkInHand.partyController.assignedTable.ResetTable());
        isCarryingCheck = false;
        armForCheckPresenters.transform.gameObject.SetActive(false);
        checkInHand.SignReceiptTipAndLeave();
    }
}
