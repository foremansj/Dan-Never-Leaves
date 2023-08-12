using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    
    [SerializeField] GameObject linkedTable;
    int maxCustomers;
    int currentCustomers;

    //bool isDirty;
    //bool isOccupied = false;

    void Start()
    {
        //tableNumber = int.Parse(this.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetTableNumber()
    {
        return linkedTable.GetComponent<TableInfo>().GetTableNumber();
    }

    public int GetMaxCustomers()
    {
        return linkedTable.GetComponent<TableInfo>().GetMaxCustomers();
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

    public void DebugCheck()
    {
        Debug.Log("Table Number = " + GetTableNumber());
        Debug.Log("Max Customers = " + GetMaxCustomers());
    }
}
