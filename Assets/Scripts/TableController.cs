using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField] int maxCustomers;
    [SerializeField] int tableNumber;
    [SerializeField] List<GameObject> seats;
    //public Dictionary<GameObject, String> customerOrders;
    public PartyController currentParty;
    Dictionary<int, List<MenuItemSO>> ordersBySeatNumber = new Dictionary<int, List<MenuItemSO>>();
    //GameObject checkHolder;
    //GameObject orderHolder;

    public bool hasCustomersSeated;
    PartyController partyController;

    void Awake()
    {
        //checkHolder = this.transform.Find("Check Holder").gameObject;
        //orderHolder = this.transform.Find("Order Holder").gameObject;
    }

    void Start()
    {
        // make sure seats are named correctly and in order
        maxCustomers = seats.Count;
        for(int i = 0; i < seats.Count; i++)
        {
            seats[i].name = "Seat #" + (i + 1);
        }
    }
    
    public int GetMaxCustomers()
    {
        return maxCustomers;
    }

    public GameObject GetSeatAtSeatNumber(int seatNumber)
    {
        return seats[seatNumber];
    }

    public int GetTableNumber()
    {
        return tableNumber;
    }

    /*public GameObject GetCheckHolder()
    {
        return checkHolder;
    }*/

    public void SetActiveParty(PartyController party)
    {
        currentParty = party;
    }

    public PartyController GetCurrentParty()
    {
        return currentParty;
    }

    public void AddOrderBySeatNumber(int seat, List<MenuItemSO> order)
    {
        ordersBySeatNumber.Add(seat, order);
    }

    public List<MenuItemSO> GetOrderBySeatNumber(int seat)
    {
        if(hasCustomersSeated)
        {
            return ordersBySeatNumber[seat];
        }
        else
        {
            return null;
        }
    }
}
