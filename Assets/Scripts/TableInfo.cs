using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableInfo : MonoBehaviour
{
    [SerializeField] private int maxCustomers;
    [SerializeField] int tableNumber;
    GameObject checkHolder;
    GameObject orderHolder;

    void Awake()
    {
        checkHolder = this.transform.Find("Check Holder").gameObject;
        orderHolder = this.transform.Find("Order Holder").gameObject;
    }
    
    public int GetMaxCustomers()
    {
        return maxCustomers;
    }

    public int GetTableNumber()
    {
        return tableNumber;
    }

    public GameObject GetCheckHolder()
    {
        return checkHolder;
    }

    public GameObject GetOrderHolder()
    {
        return orderHolder;
    }
}
