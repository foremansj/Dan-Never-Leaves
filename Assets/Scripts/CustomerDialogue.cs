using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    [SerializeField] ServerNotes serverNotes;
    [SerializeField] TextMeshProUGUI orderingDialogueText;
    [SerializeField] float typewriterEffectDelay;
    //[SerializeField] float nextCustomerOrderDelay;
    [SerializeField] GameObject player;

    string orderText;
    public int currentCustomerIndex;

    Coroutine dialogueCoroutine;
    TableController tableController;
    CameraController cameraController;
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
            orderText += order[i].ToString() + "\n";
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
        dialogueCoroutine = StartCoroutine(TypewriteOrder(order));
    }

    public void MoveToNextCustomer()
    {
        StopCoroutine(dialogueCoroutine);
        tableController = player.GetComponent<PlayerInteraction>().lastTableTouched;
        List<GameObject> seatedCustomers;
        if(tableController != null)
        {
            seatedCustomers = tableController.currentParty.partyCustomers;
        }
        else
        {
            return;
        }
        
        if(currentCustomerIndex < seatedCustomers.Count - 1)
        {
            currentCustomerIndex += 1;
            CustomerController customer = seatedCustomers[currentCustomerIndex].GetComponent<CustomerController>();
            cameraController.HardLookAtObject(customer.GetCustomerHead());
            GenerateOrderDialogue(customer.GetFullCustomerOrder());
            customer.SetHasOrdered();
            dialogueCoroutine = StartCoroutine(TypewriteOrder(orderText));
            serverNotes.nextCustomerButtonText.text = "Next Customer";
            CheckNextCustomerButton(currentCustomerIndex, seatedCustomers.Count); //check if this works
        }
        
        else
        {
            serverNotes.gameObject.SetActive(false);
            cameraController.SwitchCameras();
            player.GetComponent<PlayerInteraction>().playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void CheckNextCustomerButton(int nextCustomer, int partySize)
    {
        if(nextCustomer == partySize - 1)
        {
            serverNotes.nextCustomerButtonText.text = "Finish Taking Order";
        }
    }
}
