using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody playerRigidbody;
    [SerializeField] float walkingSpeed = 1f;
    [SerializeField] float sprintingSpeed = 10f;
    [SerializeField] Canvas orderCanvas;
    float currentSpeed;
    bool isSprinting = false;
    bool isTouchingTable = false;
    bool isTakingOrder = false;
    bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        if(value.isPressed)
        {
            Debug.Log(value.isPressed);
            isSprinting = true;
        }
        
        if(!value.isPressed)
        {
            Debug.Log("this is not pressed");
            isSprinting = false;
        }
    }

    void PlayerMovement()
    {
        if(isSprinting)
        {
            currentSpeed = sprintingSpeed;
        }
        else
        {
            currentSpeed = walkingSpeed;
        }
        
        Vector3 playerVelocity = new Vector3(moveInput.x * currentSpeed, 0, moveInput.y * currentSpeed);
        playerRigidbody.velocity = playerVelocity;
    }
    void OnInteract(InputValue value)
    {
        if(value != null && isTouchingTable == true)
        {
            TakeOrder();
        }
    }

    //finish taking order?
    //enter button ends order, closes window
    
    void TakeOrder()
    {
        if(!isTouchingTable)
        {
            //close order canvas
            orderCanvas.gameObject.SetActive(false);
            return;
        }
        else
        {
            //stop movement
            //open order canvas
            //change camera angle
            //open player writing text box
            orderCanvas.gameObject.SetActive(true);
            Debug.Log("Taking Order");
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            isTouchingTable = true;
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.transform.tag == "Tables")
        {
            isTouchingTable = false;
        }
    }
}
