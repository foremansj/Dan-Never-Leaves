using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KitchenWindowController : MonoBehaviour
{
    [SerializeField] List<GameObject> FreeKitchenWindowSlots;
    [SerializeField] List<GameObject> OccupiedKitchenWindowSlots;

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

    public IEnumerator StartCookingTicket(Dictionary<MenuItemSO, int> ticket, float ticketTime)
    {
        //wait for the ticket time to expire
        yield return new WaitForSeconds(ticketTime);
        //then instantiate the ticket items on the kitchen window in the next available spot
        yield return null;
    }
}
