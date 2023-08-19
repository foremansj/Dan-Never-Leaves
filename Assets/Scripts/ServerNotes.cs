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

    public Dictionary<string, string> workingTableNotes;
    public Dictionary<string, string> oldTableNotes;
    public Dictionary<string, string> allTableNotes;
   
    CustomerController customerController;
    CameraController cameraController;

    private void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
        workingTableNotes = new Dictionary<string, string>();
        oldTableNotes = new Dictionary<string, string>();
        allTableNotes = new Dictionary<string, string>();
        gameObject.SetActive(false);
    }

    private void Start()
    {

    }
    
    public void OpenTableNotes(TableController table)
    {
        SetNotesTableNumber(table.gameObject);
        if (workingTableNotes.ContainsKey(table.name))
        {
            Debug.Log("Opening notes for this key " + table.name);
            serverNotesInputField.text = workingTableNotes[table.name];
        }

        else
        {
            serverNotesInputField.text = "";
        }
    }

    public void SetNotesTableNumber(GameObject table)
    {
        string tableNumberOnly = Regex.Replace(table.name, "[^0-9]", "");
        notesTableHeaderText.text = "Table #" + tableNumberOnly;
    }

    public void SaveTableNotes()
    {
        TableController table = player.GetComponent<PlayerInteraction>().GetTableTouched();
        if(workingTableNotes.ContainsKey(table.name))
        {
            workingTableNotes[table.name] = serverNotesInputField.text;
        }
        
        else
        {
            workingTableNotes.Add(table.name, serverNotesInputField.text);
        }

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
