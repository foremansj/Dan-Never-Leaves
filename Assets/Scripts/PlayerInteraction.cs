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
    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] CinemachineVirtualCamera hardLookCamera;
    
    GameObject tableTouched; 
    GameObject newLookAtTargetObject;

    bool isTouchingPOS = false;

    void Start()
    {
       playerInput = GetComponent<PlayerInput>();
    }

    void OnInteract(InputValue value)
    {
        if(value != null && tableTouched != null)
        {
            orderingCanvas.SetActive(true);
            serverNotes.OpenTableNotes(tableTouched);
            SwitchCameras();
            LookAtObject(tableTouched.GetComponent<TableController>().GetCustomerAtSeat(0));
            playerInput.SwitchCurrentActionMap("Taking Orders");
            Debug.Log("Action Map =" + playerInput.currentActionMap);
        }

        if(value != null && isTouchingPOS == true)
        {
            playerInput.SwitchCurrentActionMap("UI");
            SwitchCameras();
            LookAtObject(pOSController.gameObject);
            pOSController.enabled = true;
            pOSController.OpenFloorMap();
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            tableTouched = other.gameObject;
        }

        if(other.transform.tag == "POS")
        {
            isTouchingPOS = true;
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

    public void SwitchCameras()
    {
        if(followCamera.Priority > hardLookCamera.Priority)
        {
            followCamera.Priority = 0;
            hardLookCamera.Priority = 1;
        }

        else
        {
            followCamera.Priority = 1;
            hardLookCamera.Priority = 0;
        }
    }

    public void LookAtObject(GameObject lookObject)
    {
        hardLookCamera.LookAt = lookObject.transform;
    }

    public GameObject GetTableTouched()
    {
        return tableTouched;
    }
}
