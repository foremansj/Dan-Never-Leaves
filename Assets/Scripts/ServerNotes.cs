using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ServerNotes : MonoBehaviour
{
    [SerializeField] TMP_InputField serverNotesInputField;
    [SerializeField] GameObject player;
    [SerializeField] TextMeshProUGUI notesTableHeaderText;
    [SerializeField] Button saveNotesButton;
    [SerializeField] GameObject tablesideNotes;
    [SerializeField] GameObject notesWithToast;

    public Dictionary<string, string> activeTableNotes;
    public Dictionary<string, string> oldTableNotes;
    public Dictionary<string, string> allTableNotes;
   
    CustomerController customerController;

    private void Awake()
    {
        activeTableNotes = new Dictionary<string, string>();
        oldTableNotes = new Dictionary<string, string>();
        allTableNotes = new Dictionary<string, string>();
    }

    private void Start()
    {
        //currentTableNotes = new Dictionary<string, int>();
    }
    
    public void OpenTableNotes(GameObject table)
    {
        notesTableHeaderText.text = "Table #" + table.name;        
        if(activeTableNotes.ContainsKey(table.name))
        {
            Debug.Log("Opening notes for this key " + table.name);
            serverNotesInputField.text = activeTableNotes[table.name];
        }

        else
        {
            serverNotesInputField.text = "";
        }
    }
    
    public void SaveTableNotes()
    {
        GameObject table = player.GetComponent<PlayerInteraction>().GetTableTouched();
        if(activeTableNotes.ContainsKey(table.name))
        {
            activeTableNotes[table.name] = serverNotesInputField.text;
        }
        
        else
        {
            activeTableNotes.Add(table.name, serverNotesInputField.text);
        }

        serverNotesInputField.text = "";
        gameObject.SetActive(false);
        player.GetComponent<PlayerInteraction>().SwitchCameras();
        player.GetComponent<PlayerInteraction>().playerInput.SwitchCurrentActionMap("Player");
    }

    public void DeleteTableNotes()
    {
        //rename the saved notes string to something indicating it is old
        //add a strikethrough to all the text to make it look crossed out
        //move the string so it does not come up when interacting with the table in the future
    }

    void OnEnterKey()
    {
        Debug.Log("please do something");
    }

    void NormalizeServerNotes()
    {

    }
    //when player hits ENTER, move cursor to a new line, and look at next customer
    //if there are no more customers, pan back and unfocus on customers


}
