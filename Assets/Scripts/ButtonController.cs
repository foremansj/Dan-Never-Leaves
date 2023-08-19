using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] List<GameObject> buttonCanvases;
    [SerializeField] GameObject appetizerButtons;
    [SerializeField] GameObject soupAndSaladButtons;
    [SerializeField] GameObject entreeButtons;
    [SerializeField] GameObject sideDishButtons;
    [SerializeField] GameObject drinkButtons;
    [SerializeField] GameObject dessertButtons;
    [SerializeField] GameObject kidsMenuButtons;
    [SerializeField] GameObject retailButtons;
    POSController pOSController;

    private void Awake()
    {
        pOSController = FindObjectOfType<POSController>();
    }

    public void OpenAppetizers()
    {
        CloseMenuCanvases();
        appetizerButtons.SetActive(true);
    }

    public void OpenSoupAndSalad()
    {
        CloseMenuCanvases();
        soupAndSaladButtons.SetActive(true);
    }

    public void OpenEntrees()
    {
        CloseMenuCanvases();
        entreeButtons.SetActive(true);

    }
    public void OpenSideDishes()
    {
        CloseMenuCanvases();
        sideDishButtons.SetActive(true);
    }

    public void OpenDrinks()
    {
        CloseMenuCanvases();
        drinkButtons.SetActive(true);
    }

    public void OpenDesserts()
    {
        CloseMenuCanvases();
        dessertButtons.SetActive(true);
    }

    public void OpenKidsMenu()
    {
        CloseMenuCanvases();
        kidsMenuButtons.SetActive(true);
    }

    public void OpenRetail()
    {
        CloseMenuCanvases();
        retailButtons.SetActive(true);
    }

    private void CloseMenuCanvases()
    {
        for (int i = 0; i < buttonCanvases.Count; i++)
        {
            buttonCanvases[i].SetActive(false);
        }
    }
}
