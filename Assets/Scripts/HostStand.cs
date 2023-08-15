using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HostStand : MonoBehaviour
{
    [Header("Waiting List")]
    public List<GameObject> partiesWaitingToBeSeated;
    [SerializeField] float seatingDelay;

    [Header("Table Tracking")]
    [SerializeField] List<TableController> openTablesList;
    public List<TableController> occupiedTablesList;

    PartySpawner partySpawner;

    bool isSeatingCustomers = true;

    private void Awake()
    {
        partySpawner = FindObjectOfType<PartySpawner>();
    }

    private void Start()
    {
        StartCoroutine(SeatFromWaitingList());
    }

    private void Update()
    {
        
    }
    public List<TableController> GetOpenTablesList()
    {
        return openTablesList;
    }

    IEnumerator SeatFromWaitingList()
    {   
        while(isSeatingCustomers)
        {
            if(partiesWaitingToBeSeated.Count >= 1)
            {
                for(int i = 0; i < partiesWaitingToBeSeated.Count; i++)
                {
                    Debug.Log("Seating Customers");
                    PartyController party = partiesWaitingToBeSeated[i].GetComponent<PartyController>();
                    TableController table = FindATable(party);
                    if(table != null)
                    {
                        SeatTable(party, table);
                        yield return new WaitForSeconds(seatingDelay);
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
            yield return null;
        }
    } 

    TableController FindATable(PartyController party)
    {
        int index = openTablesList.FindIndex(table => table.GetMaxCustomers() == party.GetPartySize());
        if(index >= 0)
        {
            Debug.Log("table index =" + index);
            return openTablesList[index];
        }
        else
        {
            index = openTablesList.FindIndex(table => table.GetMaxCustomers() == (party.GetPartySize() + 1));
            if(index >= 0)
            {
                Debug.Log("table index =" + index);
                return openTablesList[index];
            }
            else
            {
                return null;
            }
        }
    }
    
    void SeatTable(PartyController party, TableController table)
    {
        partiesWaitingToBeSeated.Remove(party.gameObject);
        occupiedTablesList.Add(table);
        //party.tableDestination = table;
        party.AssignTable(table);
        party.SeatParty();
                
        openTablesList.Remove(table);
        return;
    }

    public void ReopenTableForSeating (TableController table)
    {
        openTablesList.Add(table);
        int index = occupiedTablesList.IndexOf(table);
        occupiedTablesList.RemoveAt(index);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Customer")
        {
            //outdated where host stand moved party, instead of party moving party based on host stand input
            /*CustomerController customer = other.gameObject.GetComponent<CustomerController>();
            PartyController party = customer.GetComponent<PartyController>();
            if(!party.GetHasTable())
            {
                SeatTable(customer);
            }*/
        }
    }

    
}
