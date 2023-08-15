using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POSTableController : MonoBehaviour
{
    
    [SerializeField] GameObject linkedTable;
    int maxCustomers;
    int currentCustomers;
    public List<GameObject> currentParty;

    //bool isDirty;
    //bool isOccupied = false;

    public int GetTableNumber()
    {
        return linkedTable.GetComponent<TableController>().GetTableNumber();
    }

    public int GetMaxCustomers()
    {
        return linkedTable.GetComponent<TableController>().GetMaxCustomers();
    }

    public int GetCurrentCustomers()
    {
        return currentCustomers;
    }

    public GameObject GetLinkedTable()
    {
        return linkedTable;
    }

    public void AddCustomer()
    {
        currentCustomers++;
    }

    public void CloseTable()
    {
        currentCustomers = 0;
        //move customer check to closed checks
        //isOccupied = false;
    }

    public void SitTable()
    {
        //isOccupied = true;
    }

    public void ResetTable()
    {
        //isDirty = false;
    }
}
