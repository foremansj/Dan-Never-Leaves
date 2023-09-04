using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ServerNotes : MonoBehaviour
{
    [SerializeField] public TMP_InputField serverNotesInputField;
    [SerializeField] GameObject player;
    [SerializeField] public TextMeshProUGUI notesTableHeaderText;
    [SerializeField] Button saveNotesButton;
    [SerializeField] public TextMeshProUGUI nextCustomerButtonText;
    [SerializeField] GameObject tablesideNotes;
    [SerializeField] GameObject notesWithToast;

    public Dictionary<int, string> workingTableNotes;
    //public Dictionary<int, string> oldTableNotes;
    //public Dictionary<int, string> allTableNotes;
    int activeTableNumber; 
   
    CameraController cameraController;

    private void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
        workingTableNotes = new Dictionary<int, string>();
        gameObject.SetActive(false);
    }

    public void OpenTableNotes(TableController table)
    {
        activeTableNumber = table.GetTableNumber();
        notesTableHeaderText.text = "Table #" + activeTableNumber;
        if (workingTableNotes.ContainsKey(table.GetTableNumber()))
        {
            serverNotesInputField.text = workingTableNotes[table.GetTableNumber()];
        }

        else
        {
            workingTableNotes.Add(activeTableNumber, "");
            serverNotesInputField.text = "";
        }
    }

    public void SaveTableNotes()
    {
        workingTableNotes[activeTableNumber] = serverNotesInputField.text;
    }

    public void CloseTableNotes()
    {
        //this used to be part of SaveTableNotes
        serverNotesInputField.text = "";
        gameObject.SetActive(false);
        cameraController.SwitchCameras();
        player.GetComponent<PlayerInteraction>().playerInput.SwitchCurrentActionMap("Player");
    }

    public void DeleteTableNotes(int tableNumber)
    {
        tablesideNotes.GetComponent<ServerNotes>().workingTableNotes[tableNumber] = null;
        notesWithToast.GetComponent<ServerNotes>().workingTableNotes[tableNumber] = null;
    }

    //when player hits ENTER, move cursor to a new line, and look at next customer
    //if there are no more customers, pan back and unfocus on customers
}
