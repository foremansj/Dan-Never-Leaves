using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    [SerializeField] Rigidbody rb;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintBoost;
    //[SerializeField] Camera firstPersonCamera;
    PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        Quaternion cameraRotation = Camera.main.transform.rotation;
        gameObject.transform.rotation = new Quaternion(0, cameraRotation.y, 0, cameraRotation.w);
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

    }
    
    void HandleMovement()
    {
        InputAction sprint = playerInput.actions["Sprint"];
        float xSpeed = moveInput.x * walkSpeed * Time.deltaTime;
        float zSpeed = moveInput.y * walkSpeed * Time.deltaTime;

        if(sprint.IsInProgress())
        {
            //rb.AddRelativeForce(moveInput.x * walkSpeed * Time.deltaTime, 0, moveInput.y * walkSpeed * Time.deltaTime, ForceMode.Force); //Mathf.Sign() ??
            rb.transform.Translate(xSpeed * sprintBoost, 0, zSpeed * sprintBoost);
            //Debug.Log("Sprinting");
        }

        else
        {
            //rb.AddRelativeForce(moveInput.x * walkSpeed * Time.deltaTime, 0, moveInput.y * walkSpeed * Time.deltaTime, ForceMode.Force);
            rb.transform.Translate(xSpeed, 0, zSpeed);
            //Debug.Log("NOT Sprinting");
            //Debug.Log("Speed is" + Vector3.Magnitude(rb.velocity));
        }
        
    }
}
