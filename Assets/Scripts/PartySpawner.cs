using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySpawner : MonoBehaviour
{
    HostStand hostStand;
    
    [Header("Customer Spawning")]
    [SerializeField] GameObject customerPrefab;
    [SerializeField] Vector3 spawnPointOrigin;
    [SerializeField] float customerSpawnDelay;

    [SerializeField] bool isOpenForBusiness = true;

    private void Awake()
    {
        hostStand = FindObjectOfType<HostStand>();
    }

    private void Start()
    {
        StartCoroutine(CreateNewParty());
    }

    private void Update()
    {
        SetCustomerSpawnSpeed();
    }

    IEnumerator CreateNewParty()
    {
        do
        {
            startOver:
                int partySize = Random.Range(2,6);
                GameObject newParty = new GameObject("Party of " + partySize);
                newParty.AddComponent<PartyController>();
                for(int i = 0; i < partySize; i++)
                {
                    Vector3 spawnPoint = new Vector3(spawnPointOrigin.x + i, spawnPointOrigin.y, spawnPointOrigin.z + i);
                    GameObject newCustomer = Instantiate(customerPrefab, spawnPoint, Quaternion.identity);
                    newCustomer.transform.SetParent(newParty.transform);  
                }
                
                yield return new WaitForSeconds(customerSpawnDelay);
                goto startOver;
        } while(isOpenForBusiness);
    }

    void SetCustomerSpawnSpeed()
    {
        switch(Time.time)
        {
            case float n when (n <= 60):
                customerSpawnDelay = 5f;
                //isOpenForBusiness = true;
                break;

            case float n when (n > 60 && n <= 120):
                customerSpawnDelay = 15f;
                //isOpenForBusiness = true;
                break;
            
            case float n when (n > 120 && n <= 180):
                customerSpawnDelay = 15f;
                //isOpenForBusiness = true;
                break;
            
            case float n when (n > 180 && n <= 240):
                customerSpawnDelay = 15f;
                //isOpenForBusiness = true;
                break;
            
            case float n when (n > 240 && n <= 300):
                customerSpawnDelay = 15f;
                //isOpenForBusiness = false;
                break;
            
            default:
                customerSpawnDelay = 100000f;
                StopCoroutine(CreateNewParty());
                break;
        }
    }

    public bool GetIsOpenForBusiness()
    {
        return isOpenForBusiness;
    }
}