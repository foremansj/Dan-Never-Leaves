using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] GameObject notepadPrefab;
    [SerializeField] POSController pOSController;
    
    bool isTouchingTable = false;
    bool isTouchingPOS = false;

    string tableNumber; 

    PlayerInput playerInput;
    
    void Start()
    {
       playerInput = GetComponent<PlayerInput>();
    }

    void OnInteract(InputValue value)
    {
        if(value != null && isTouchingTable == true)
        {
            playerInput.SwitchCurrentActionMap("UI");
            TakeOrder();
        }

        if(value != null && isTouchingPOS == true)
        {
            pOSController.OpenFloorMap();
        }
    }

    //finish taking order?
    //enter button ends order, closes window
    
    void TakeOrder()
    {
        if(!isTouchingTable)
        {
            //close order canvas
            //orderCanvas.gameObject.SetActive(false);
            return;
        }
        else
        {
            //stop movement
            playerInput.SwitchCurrentActionMap("UI");
            
            //check if table already has notes
            //if it does, open those up
            //if not, create new notes
            
            Instantiate(notepadPrefab, new Vector3(0,0,0), Quaternion.identity);
        }
    }

    public void SaveOrderNotes()
    {
        //save the notes for that table
        //store them on the table?
        //close the order notepad
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            isTouchingTable = true;
            tableNumber = other.name;
        }

        if(other.transform.tag == "POS")
        {
            isTouchingPOS = true;
            Debug.Log("touching POS");
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            isTouchingTable = false;
        }
        
        if(other.transform.tag == "POS")
        {
            isTouchingPOS = false;
        }
    }
}
