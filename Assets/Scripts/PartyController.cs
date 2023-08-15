using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PartyController : MonoBehaviour
{
    [Header("Orders & Menu")]
    public List<MenuItemSO> fullPartyOrder;
    [SerializeField] List<MenuItemSO> appetizers;
    [SerializeField] List<MenuItemSO> soupsAndSalads;
    [SerializeField] List<MenuItemSO> entrees;
    [SerializeField] List<MenuItemSO> desserts;
    [SerializeField] List<MenuItemSO> drinks;

    [Header("Host & Seating")]
    public TableController tableDestination;
    public List<GameObject> partyCustomers;
    public List<GameObject> seatsAtTable;
    [SerializeField] bool hasTable;
    HostStand hostStand;

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

        hostStand.partiesWaitingToBeSeated.Add(gameObject);
    }

    private void Update()
    {

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
            GameObject seat = tableDestination.GetCustomerAtSeat(i);
            seatsAtTable.Add(seat);
            CustomerController customer = partyCustomers[i].GetComponent<CustomerController>();
            customer.customerSeat = seat;
            customer.MoveToDestination(seat.transform);
        }
    }
    
    public void AddToOrder(MenuItemSO item)
    {
        fullPartyOrder.Add(item);
    }

    public List<MenuItemSO> GetFullPartyOrder()
    {
        return fullPartyOrder;
    }

    public List<MenuItemSO> GetAppetizers()
    {
        return appetizers;
    }

    public List<MenuItemSO> GetSoupsAndSalads()
    {
        return soupsAndSalads;
    }

    public List<MenuItemSO> GetEntrees()
    {
        return entrees;
    }

    public List<MenuItemSO> GetDesserts()
    {
        return desserts;
    }

    public List<MenuItemSO> GetDrinks()
    {
        return drinks;
    }
}
