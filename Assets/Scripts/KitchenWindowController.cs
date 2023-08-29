using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class KitchenWindowController : MonoBehaviour
{
    [SerializeField] List<GameObject> freeKitchenWindowSlots;
    [SerializeField] List<GameObject> occupiedKitchenWindowSlots;
    [SerializeField] GameObject foodPrefab;
    [SerializeField] GameObject kitchenWindow;
    [SerializeField] TextMeshProUGUI handsText;
    [SerializeField] float handsTextDuration = 500f;

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
        yield return new WaitForSeconds(ticketTime);
        ResetOpenWindowSlots();
        GameObject openWindow = FindOpenWindowTransform();
        Vector3 spawnPosition = openWindow.transform.position;
        if(spawnPosition != null)
        {
            StartCoroutine(CallForHands(tableNumber));
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

    public IEnumerator CallForHands(int table)
    {
        handsText.text = "HANDS Table " + table;
        handsText.enabled = true;
        yield return new WaitForSeconds(handsTextDuration * Time.deltaTime);
        handsText.enabled = false;
    }
}
