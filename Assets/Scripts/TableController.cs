using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TableController : MonoBehaviour
{
    [SerializeField] int maxCustomers;
    [SerializeField] int tableNumber;
    [SerializeField] List<GameObject> seats;
    [SerializeField] GameObject foodPrefab;

    public PartyController currentParty;
    Dictionary<int, List<MenuItemSO>> ordersBySeatNumber = new Dictionary<int, List<MenuItemSO>>();

    public bool hasCustomersSeated;
    public bool isFoodDropped;
    public bool isFinishedEating = false;
    public bool isCheckDropped;
    //PartyController partyController;

    void Awake()
    {

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

    public void PlaceFoodForCustomers()
    {
        isFinishedEating = false;
        isFoodDropped = true;
        for(int i = 0; i < currentParty.GetPartySize(); i++)
        {
            Vector3 placeSetting = seats[i].transform.GetChild(0).transform.position;
            GameObject food = Instantiate(foodPrefab, placeSetting, Quaternion.identity);
            food.transform.parent = seats[i].transform;
            food.transform.LookAt(seats[i].transform);
            food.GetComponentInChildren<TextMeshProUGUI>().text = currentParty.partyCustomers[i].GetComponent<CustomerController>().mainCourse.itemName;
            //start the customer eating clock and food fill
            CustomerController customerAtSeat = currentParty.partyCustomers[i].GetComponent<CustomerController>();

            food.transform.GetChild(0).GetChild(0).transform.gameObject.SetActive(true);
            //food.transform.Find("Slider").transform.gameObject.SetActive(true);
            //Slider foodSlider = food.GetComponentInChildren<Slider>();
            //foodSlider.SetEnabled(true);
            //customerAtSeat.EatFoodOnTable(food);
            StartCoroutine(customerAtSeat.EatFoodOnTable(food));
        }
    }

    public void RemovePlatesFromTable()
    {
        for(int i = 0; i < currentParty.GetPartySize(); i++)
        {
            GameObject food = seats[i].transform.GetChild(1).gameObject;
            Destroy(food);
        }
    }

    public bool GetIsTableStillEating()
    {
        for(int i = 0; i < currentParty.partyCustomers.Count; i++)
        {
            if(currentParty.partyCustomers[i].GetComponent<CustomerController>().GetIsEating() == false)
            {
                return false;
            }
        }
        return true;
    }
}
