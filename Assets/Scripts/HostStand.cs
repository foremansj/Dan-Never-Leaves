using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HostStand : MonoBehaviour
{
    [Header("Waiting List")]
    public List<GameObject> partiesWaitingToBeSeated;
    [SerializeField] float seatingDelay;
    [SerializeField] GameObject waitingArea;

    [Header("Table Tracking")]
    [SerializeField] List<TableController> openTablesList;
    public List<TableController> occupiedTablesList;

    PartySpawner partySpawner;

    bool isSeatingCustomers = true;

    private void Awake()
    {
        partySpawner = FindObjectOfType<PartySpawner>();
        //partiesWaitingToBeSeated = new List<GameObject>();
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
            if(partiesWaitingToBeSeated != null)
            {
                for(int i = 0; i < partiesWaitingToBeSeated.Count; i++)
                {
                    PartyController party = partiesWaitingToBeSeated[i].GetComponent<PartyController>();
                    TableController table = FindATable(party);
                    
                    if(table != null)
                    {
                        SeatTable(party, table);
                        party.GetComponent<CheckController>().tableNumber = table.GetTableNumber();
                        yield return new WaitForSeconds(seatingDelay);
                    }
                    
                    else
                    {
                        SendPartyToWaitingArea(party);
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
            return openTablesList[index];
        }
        else
        {
            index = openTablesList.FindIndex(table => table.GetMaxCustomers() == (party.GetPartySize() + 1));
            if(index >= 0)
            {
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
        occupiedTablesList.Add(table);
        openTablesList.Remove(table);
        party.AssignTable(table);
        party.SeatParty();
        partiesWaitingToBeSeated.Remove(party.gameObject);
        table.SetActiveParty(party);

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

    public void SendPartyToWaitingArea(PartyController party)
    {
        for(int i = 0; i < party.GetPartySize(); i++)
        {
            CustomerController customer = party.partyCustomers[i].GetComponent<CustomerController>();
            customer.MoveToDestination(waitingArea.transform);
        }
    }
}
