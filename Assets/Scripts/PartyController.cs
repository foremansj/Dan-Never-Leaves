using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PartyController : MonoBehaviour
{
    //[Header("Orders & Menu")]
    //public List<MenuItemSO> fullPartyOrder = new List<MenuItemSO>();
    //public Dictionary<MenuItemSO, int> partyOrderDictionary = new Dictionary<MenuItemSO, int>();

    [Header("Host & Seating")]
    public TableController tableDestination;
    public List<GameObject> partyCustomers;
    public List<GameObject> seatsAtTable;
    [SerializeField] bool hasTable;
    HostStand hostStand;
    CheckController checkController;

    private void Awake()
    {
        hostStand = FindObjectOfType<HostStand>();
        partyCustomers = new List<GameObject>();
        seatsAtTable = new List<GameObject>();
    }

    private void Start()
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            partyCustomers.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).gameObject.name = "Customer #" + (i + 1);
        }
        CreateCheck();
        hostStand.partiesWaitingToBeSeated.Add(gameObject);
    }

    void CreateCheck()
    {
        checkController = this.AddComponent<CheckController>();
    }

    public void AddCustomersToParty(GameObject newCustomer)
    {
        partyCustomers.Add(newCustomer);
        newCustomer.name = "Customer #" + (partyCustomers.IndexOf(newCustomer) + 1);
    }

    public bool GetHasTable()
    {
        return hasTable;
    }

    public TableController GetTableDestination()
    {
        return tableDestination;
    }

    public int GetPartySize()
    {
        return partyCustomers.Count;
    }

    public void AssignTable(TableController table)
    {
        tableDestination = table;
        hasTable = true;
        table.SetActiveParty(gameObject.GetComponent<PartyController>());
    }

    public void SeatParty()
    {
        for(int i = 0; i < partyCustomers.Count; i++)
        {
            GameObject seat = tableDestination.GetSeatAtSeatNumber(i);
            seatsAtTable.Add(seat);
            CustomerController customer = partyCustomers[i].GetComponent<CustomerController>();
            customer.customerSeat = seat;
            
            tableDestination.AddOrderBySeatNumber(i, customer.GetFullCustomerOrder());
            customer.AddOrderToCheck();
            
            customer.MoveToDestination(seat.transform);
        }
        checkController.ListOutOrder();
        checkController.CalculateCheckTotal();
    }
    
    /*public void AddItemToOrder(MenuItemSO item)
    {
        int itemCount;
        if(partyOrderDictionary.ContainsKey(item))
        {
            partyOrderDictionary.TryGetValue(item, out itemCount);
            partyOrderDictionary[item] = itemCount + 1;
        }
        else
        {
            partyOrderDictionary.Add(item, 1);
        }
    }*/

    
}
