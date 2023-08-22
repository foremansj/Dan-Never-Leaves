using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



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

    [SerializeField] float eatingDuration;
    public float eatingSpeedModifier;
    bool isEating;

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

        eatingDuration = Random.Range(35f, 55f);
        
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

    public IEnumerator EatFoodOnTable(GameObject food)
    {
        
        Slider foodSlider = food.transform.gameObject.GetComponentInChildren<Slider>();
        foodSlider.minValue = 0f;
        foodSlider.maxValue = eatingDuration;
        isEating = true;
        
        bool isLooping = true;
        while(isLooping)
        {
            foodSlider.value += Time.deltaTime;
            if(foodSlider.value >= eatingDuration)
            {
                isLooping = false;
                isEating = false;
                yield return null;
            }
            yield return null;
        }
    }

    public bool GetIsEating()
    {
        return isEating;
    }
    

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
