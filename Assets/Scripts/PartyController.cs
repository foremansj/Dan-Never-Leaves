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
    public TableController assignedTable;
    public List<GameObject> partyCustomers;
    public List<GameObject> seatsAtTable;
    [SerializeField] bool hasTable;
    HostStand hostStand;
    CheckController checkController;

    public float paymentDelay;
    public bool hasFullPartyOrdered = false;
    public bool isReadyForCheck = false;
    public bool hasDroppedCreditCard = false;

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
    
    private void Update()
    {
        CheckIfPartyHasOrdered();
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
        return assignedTable;
    }

    public int GetPartySize()
    {
        return partyCustomers.Count;
    }

    public void AssignTable(TableController table)
    {
        assignedTable = table;
        hasTable = true;
        table.SetActiveParty(gameObject.GetComponent<PartyController>());
    }

    public void SeatParty()
    {
        for(int i = 0; i < partyCustomers.Count; i++)
        {
            GameObject seat = assignedTable.GetSeatAtSeatNumber(i);
            seatsAtTable.Add(seat);
            CustomerController customer = partyCustomers[i].GetComponent<CustomerController>();
            customer.customerSeat = seat;
            
            assignedTable.AddOrderBySeatNumber(i, customer.GetFullCustomerOrder());
            customer.AddOrderToCheck();
            
            customer.MoveToDestination(seat.transform);
        }
        checkController.ListOutOrder();
        //checkController.CalculateCheckTotals();
    }

    public void CheckIfFullPartyHasSat()
    {
        int sitCounter = 0;
        for(int i = 0; i < partyCustomers.Count; i++)
        {
            if(partyCustomers[i].GetComponent<CustomerController>().isSeated)
            {
                sitCounter++;
                if(sitCounter == partyCustomers.Count)
                {
                    assignedTable.isReadyToOrder = true;
                }
            }
        }
    }
    public bool CheckIfPartyHasOrdered()
    {
        if(!hasFullPartyOrdered)
        {
            int orderCheck = 0;
            for(int i = 0; i < partyCustomers.Count; i++)
            {
                if(partyCustomers[i].GetComponent<CustomerController>().hasOrdered)
                {
                    orderCheck++;
                    if(orderCheck == partyCustomers.Count)
                    {
                        hasFullPartyOrdered = true;
                        return hasFullPartyOrdered;
                    }
                }
            }
            return hasFullPartyOrdered;
        }
        else
        {
            return true;
        }
    }

    public void LeaveRestaurant()
    {
        for(int i = 0; i < partyCustomers.Count; i++)
        {
            CustomerController customer = partyCustomers[i].GetComponent<CustomerController>();
            customer.isLeaving = true;
            customer.LeaveRestaurant();
        }
    }
}
