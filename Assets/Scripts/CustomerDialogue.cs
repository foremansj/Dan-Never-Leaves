using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    [SerializeField] ServerNotes serverNotes;
    [SerializeField] TextMeshProUGUI orderingDialogueText;
    [SerializeField] float typewriterEffectDelay;
    [SerializeField] GameObject player;

    TableController tableController;
    CameraController cameraController;
    string orderText;
    public int currentCustomerIndex;

    //bool typewriterIsRunning;
    Coroutine dialogueCoroutine;
    private void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
    }

    public void SetOrderDialogue(string order)
    {
        orderingDialogueText.text = order;
    }

    public string GenerateOrderDialogue(List<MenuItemSO> order)
    {
        orderText = "I would like the ";
        for(int i = 0; i < order.Count; i++)
        {
            orderText += (order[i].ToString() + "\n");
        }
        orderText = orderText.Replace("(MenuItemSO)", "");
        return orderText;
    }

    public string GetOrderText()
    {
        return orderText;
    }

    IEnumerator TypewriteOrder(string order)
    {
        if(order != null)
        {
            //typewriterIsRunning = true;
            orderingDialogueText.text = null;
            foreach(char letter in order)
            {
                orderingDialogueText.text += letter;
                yield return new WaitForSeconds(typewriterEffectDelay);
            }
        }
        
        yield return null;
    }

    public void StartTypewriterCoroutine(string order)
    {
        //StopCoroutine(nameof(TypewriteOrder));
        //orderingDialogueText.text = null;
        dialogueCoroutine = StartCoroutine(TypewriteOrder(order));
    }

    public void MoveToNextCustomer()
    {
        StopCoroutine(dialogueCoroutine);
        tableController = player.GetComponent<PlayerInteraction>().tableTouched;
        List<GameObject> seatedCustomers;
        if(tableController != null)
        {
            seatedCustomers = tableController.currentParty.partyCustomers;
        }
        else
        {
            return;
        }
        
        if(currentCustomerIndex + 1 < seatedCustomers.Count)
        {
            currentCustomerIndex += 1;
            CustomerController customer = seatedCustomers[currentCustomerIndex].GetComponent<CustomerController>();
            cameraController.HardLookAtObject(customer.GetCustomerHead());
            GenerateOrderDialogue(customer.GetFullCustomerOrder());
            dialogueCoroutine = StartCoroutine(TypewriteOrder(orderText));
        }

        /*else if(currentCustomerIndex + 1 == seatedCustomers.Count)
        {
            CustomerController customer = seatedCustomers[currentCustomerIndex].GetComponent<CustomerController>();
            cameraController.HardLookAtObject(customer.GetCustomerHead());
            GenerateOrderDialogue(customer.GetFullCustomerOrder());
            dialogueCoroutine = StartCoroutine(TypewriteOrder(orderText));

            //remove next customer button and/or make it save the notes
        }*/
        else
        {
            serverNotes.gameObject.SetActive(false);
            cameraController.SwitchCameras();
            player.GetComponent<PlayerInteraction>().playerInput.SwitchCurrentActionMap("Player");
        }
    }
}
