using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CustomerController : MonoBehaviour
{
    NavMeshAgent thisAgent; 
    NavMeshObstacle thisObstacle;
    Rigidbody thisRigidbody;
    public GameObject customerSeat;
    public TableController partyTable;
    HostStand hostStand;
    public PartyController party;
    MenuDatabase menuDatabase;
    [SerializeField] GameObject customerHead;
    public bool isSeated;

    [Header("Customer Order")]
    public MenuItemSO firstCourse;
    public MenuItemSO mainCourse;
    public MenuItemSO drink;
    public MenuItemSO dessert;
    public MenuItemSO retailPurchase;
    public bool hasOrdered = false;
    public bool isChild = false;
    public List<MenuItemSO> fullOrder;
    CheckController checkController;

    void Awake()
    {
        hostStand = FindObjectOfType<HostStand>();
        menuDatabase = FindObjectOfType<MenuDatabase>();
    }
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        party = transform.parent.GetComponent<PartyController>();
        thisAgent = GetComponent<NavMeshAgent>();
        thisObstacle = GetComponent<NavMeshObstacle>();
        float makeChild = Random.Range(0f, 10f);
        if(makeChild > 8.5f)
        {
            isChild = true;
        }
        
        GenerateOrder();
    }

    void Update()
    {
        partyTable = party.tableDestination;
    }

    public void MoveToDestination(Transform transform)
    {
        thisAgent.SetDestination(transform.position);
    }

    public IEnumerator moveCustomer(Transform transform, float delay)
    {
        thisAgent.SetDestination(transform.position);
        yield return new WaitForSeconds(delay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(partyTable != null)
        {
            if(other.gameObject == party.tableDestination.gameObject)
            {
                thisAgent.enabled = false;
                thisObstacle.enabled = true;
                transform.LookAt(party.tableDestination.transform);
                thisRigidbody.constraints = RigidbodyConstraints.FreezePosition;
                thisRigidbody.isKinematic = true;
                gameObject.transform.position = customerSeat.transform.position;
                isSeated = true;
                other.GetComponent<TableController>().hasCustomersSeated = true;
            }
        }
    }

    private void GenerateOrder()
    {
        if(!hasOrdered && !isChild)
        {
            bool gettingAppetizer = Random.value > 0.5f;
            bool gettingDessert = Random.value > 0.5f;
            
            drink = menuDatabase.GetRandomDrink();
            fullOrder.Add(drink);
            if(gettingAppetizer)
            {
                firstCourse = menuDatabase.GetRandomAppetizer();
                fullOrder.Add(firstCourse);
            }
            mainCourse = menuDatabase.GetRandomEntree();
            fullOrder.Add(mainCourse);
            
            if(gettingDessert)
            {
                dessert = menuDatabase.GetRandomDessert();
                fullOrder.Add(dessert);
            }
        }
        else if(!hasOrdered && isChild)
        {
            drink = menuDatabase.GetKidsDrink();
            fullOrder.Add(drink);
            mainCourse = menuDatabase.GetRandomKidsMenuItem();
            fullOrder.Add(mainCourse);

        }
    }

    public void AddOrderToCheck()
    {
        checkController = party.GetComponent<CheckController>();
        foreach(MenuItemSO item in fullOrder)
        {
            checkController.CompileFullPartyOrder(item);
        }
    }

    
    /*public void OrderFood()
    {
        hasOrdered = true;
        foreach(MenuItemSO item in fullOrder)
        {
            party.GetComponent<CheckController>().AddToCheck(item);
        }
    }*/

    void AdjustHappiness()
    {

    }

    public List<MenuItemSO> GetFullCustomerOrder()
    {
        return fullOrder;
    }

    public GameObject GetCustomerHead()
    {
        return customerHead;
    }
}
