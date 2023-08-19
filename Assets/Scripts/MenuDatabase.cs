using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDatabase : MonoBehaviour
{
    [Header("Menu Categories")]
    [SerializeField] List<MenuItemSO> appetizers;
    [SerializeField] List<MenuItemSO> soupsAndSalads;
    [SerializeField] List<MenuItemSO> entrees;
    [SerializeField] List<MenuItemSO> sides;
    [SerializeField] List<MenuItemSO> desserts;
    [SerializeField] List<MenuItemSO> drinks;
    [SerializeField] List<MenuItemSO> kidsMenu;
    [SerializeField] List<MenuItemSO> retail;

    public MenuItemSO GetRandomAppetizer()
    {
        int index = Random.Range(0, appetizers.Count);
        return appetizers[index];
    }

    public MenuItemSO GetRandomSoupOrSalad()
    {
        int index = Random.Range(0, soupsAndSalads.Count);
        return soupsAndSalads[index];
    }

    public MenuItemSO GetRandomEntree()
    {
        int index = Random.Range(0, entrees.Count);
        return entrees[index];
    }

    public MenuItemSO GetRandomSide()
    {
        int index = Random.Range(0, sides.Count);
        return sides[index];
    }

    public MenuItemSO GetRandomDrink()
    {
        int index = Random.Range(0, drinks.Count);
        return drinks[index];
    }

    public MenuItemSO GetRandomDessert()
    {
        int index = Random.Range(0, desserts.Count);
        return desserts[index];
    }

    public MenuItemSO GetRandomKidsMenuItem()
    {
        int index = Random.Range(0, kidsMenu.Count);
        return kidsMenu[index];
    }

    public MenuItemSO GetKidsDrink()
    {
        drinks.Find(item => item.name == "NA Beverage");
        return drinks.Find(item => item.name == "NA Beverage");
    }

    public MenuItemSO GetRandomRetailItem()
    {
        int index = Random.Range(0, retail.Count);
        return retail[index];
    }
    
}
