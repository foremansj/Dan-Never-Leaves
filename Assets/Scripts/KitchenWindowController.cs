using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class KitchenWindowController : MonoBehaviour
{
    [SerializeField] List<GameObject> freeKitchenWindowSlots;
    [SerializeField] List<GameObject> occupiedKitchenWindowSlots;
    [SerializeField] GameObject foodPrefab;
    [SerializeField] GameObject kitchenWindow;

    public float SetTicketTime(Dictionary<MenuItemSO, int> ticket)
    {
        float ticketTime = 0f;
        foreach(KeyValuePair<MenuItemSO, int> pair in ticket)
        {
            if(pair.Key.ticketTimeSeconds > ticketTime)
            {
                ticketTime = pair.Key.ticketTimeSeconds;
            }
        }
        return ticketTime;
    }

    public IEnumerator StartCookingTicket(int tableNumber, Dictionary<MenuItemSO, int> ticket, float ticketTime)
    {
        //wait for the ticket time to expire
        Debug.Log("ticket time = " + ticketTime);
        yield return new WaitForSeconds(ticketTime);
        Debug.Log("Food is ready for table " + tableNumber);
        ResetOpenWindowSlots();
        //yield return null;
        GameObject openWindow = FindOpenWindowTransform();
        Vector3 spawnPosition = openWindow.transform.position;
        if(spawnPosition != null)
        {
            GameObject food = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
            food.GetComponentInChildren<TextMeshProUGUI>().text = "Table " + tableNumber;
            food.name = tableNumber.ToString();
            food.transform.parent = openWindow.transform;
        }
        
        yield return null;
    }

    private GameObject FindOpenWindowTransform()
    {
        for(int i = 0; i < freeKitchenWindowSlots.Count; i++)
        {
            if(freeKitchenWindowSlots[i].transform.childCount == 0)
            {
                GameObject openWindow = freeKitchenWindowSlots[i].gameObject;
                occupiedKitchenWindowSlots.Add(freeKitchenWindowSlots[i]);
                freeKitchenWindowSlots.RemoveAt(i);
                return openWindow;
            }
        }
        return null;
    }

    public void ReopenWindowSlot(GameObject obj)
    {
        Destroy(obj.transform.GetChild(0).gameObject);
        occupiedKitchenWindowSlots.Remove(obj);
        freeKitchenWindowSlots.Add(obj);
    }

    public void ResetOpenWindowSlots()
    {
        freeKitchenWindowSlots.Clear();
        for(int i = 0; i < kitchenWindow.transform.childCount; i++)
        {
            freeKitchenWindowSlots.Add(kitchenWindow.transform.GetChild(i).gameObject);
        }
    }
}
