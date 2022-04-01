using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : NetworkBehaviour
{
    [SerializeField] public GameObject coffee;
    [SerializeField] public GameObject tea;
    [SerializeField] public GameObject energyDrink;

    private bool _initialized = false;
    [SyncVar] private PlayerCustomization.DRINK drinkSelection;

    private void Start()
    {
        // At this point only the client should be un-initialized
        if (!_initialized)
        {
            ShowDrinks();
        }
    }

    [Server]
    public void SetSelection(PlayerCustomization.DRINK selection)
    {
        drinkSelection = selection;
        ShowDrinks();
    }

    private void ShowDrinks()
    {
        coffee.SetActive(drinkSelection == PlayerCustomization.DRINK.COFFEE);
        tea.SetActive(drinkSelection == PlayerCustomization.DRINK.TEA);
        energyDrink.SetActive(drinkSelection == PlayerCustomization.DRINK.ENERGYDRINK);
    }
}
