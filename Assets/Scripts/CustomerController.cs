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
    public bool isEating;
    public bool isDoneEating;
    public bool reachedExit;
    public bool isLeaving;

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
        partyTable = party.assignedTable;
        if(reachedExit && isLeaving)
        {
            Destroy(party.gameObject,2f);
            Destroy(gameObject, 2f);
        }
        if(isLeaving)
        {
            MoveToDestination(FindObjectOfType<PartySpawner>().exitPoint.transform);
        }
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
            if(other.gameObject == party.assignedTable.gameObject)
            {
                thisAgent.enabled = false;
                thisObstacle.enabled = true;
                transform.LookAt(party.assignedTable.transform);
                thisRigidbody.constraints = RigidbodyConstraints.FreezePosition;
                thisRigidbody.isKinematic = true;
                gameObject.transform.position = customerSeat.transform.position;
                isSeated = true;
                other.GetComponent<TableController>().hasCustomersSeated = true;
                party.CheckIfFullPartyHasSat();
            }
        }

        if(other.tag == "Exit" && isLeaving)
        {
            reachedExit = true;
        }
    }

    private void GenerateOrder()
    {//additional course orders commented out for testing, add back later
        if(!hasOrdered && !isChild)
        {
            /*bool gettingAppetizer = Random.value > 0.5f;
            bool gettingDessert = Random.value > 0.5f;
            
            drink = menuDatabase.GetRandomDrink();
            fullOrder.Add(drink);
            if(gettingAppetizer)
            {
                firstCourse = menuDatabase.GetRandomAppetizer();
                fullOrder.Add(firstCourse);
            }*/
            mainCourse = menuDatabase.GetRandomEntree();
            fullOrder.Add(mainCourse);
            
            /*if(gettingDessert)
            {
                dessert = menuDatabase.GetRandomDessert();
                fullOrder.Add(dessert);
            }*/
        }
        else if(!hasOrdered && isChild)
        {
            //drink = menuDatabase.GetKidsDrink();
            //fullOrder.Add(drink);
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
                foodSlider.transform.Find("InnerBorder").GetComponent<Image>().color = new Color(0f, 0.5471698f, 0.02956277f);
                foodSlider.transform.Find("OuterBorder").GetComponent<Image>().color = new Color(0f, 0.5471698f, 0.02956277f);
                isLooping = false;
                isEating = false;
                yield return null;
            }
            yield return null;
        }
        isDoneEating = true;
        if(partyTable.CheckIfTableIsDoneEating())
        {
            partyTable.isReadyToBus = true;
        }
    }

    /*public bool GetIsEating()
    {
        return isEating;
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

    public void SetHasOrdered()
    {
        hasOrdered = true;
    }

    public void LeaveRestaurant()
    {
        thisObstacle.enabled = false;
        transform.position += new Vector3(0, 0.53f, 0);
        thisAgent.enabled = true;
    }
}
