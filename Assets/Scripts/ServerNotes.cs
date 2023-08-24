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
    [SerializeField] GameObject tablesideNotes;
    [SerializeField] GameObject notesWithToast;

    public Dictionary<int, string> workingTableNotes;
    public Dictionary<int, string> oldTableNotes;
    public Dictionary<int, string> allTableNotes;
    int activeTableNumber; 
   
    CustomerController customerController;
    CameraController cameraController;

    private void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
        workingTableNotes = new Dictionary<int, string>();
        oldTableNotes = new Dictionary<int, string>();
        allTableNotes = new Dictionary<int, string>();
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

    /*public void SetNotesTableNumber(int tableNumber)
    {
        //string tableNumberOnly = Regex.Replace(table.name, "[^0-9]", "");
        notesTableHeaderText.text = "Table #" + tableNumber;
    }*/

    /*public void OLDSaveTableNotes()
    {
        int tableNumber = player.GetComponent<PlayerInteraction>().GetTableTouched().GetTableNumber();
        if(workingTableNotes.ContainsKey(tableNumber))
        {
            workingTableNotes[tableNumber] = serverNotesInputField.text;
        }
        
        else
        {
            workingTableNotes.Add(tableNumber, serverNotesInputField.text);
        }
    }*/
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

    public void DeleteTableNotes()
    {
        //rename the saved notes string to something indicating it is old
        //add a strikethrough to all the text to make it look crossed out
        //move the string so it does not come up when interacting with the table in the future
    }

    //when player hits ENTER, move cursor to a new line, and look at next customer
    //if there are no more customers, pan back and unfocus on customers
}
