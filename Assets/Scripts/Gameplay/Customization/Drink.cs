using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : NetworkBehaviour
{
    [SerializeField] public GameObject coffee;
    [SerializeField] public GameObject tea;
    [SerializeField] public GameObject energyDrink;

    [Server]
    public void SetSelection(PlayerCustomization.DRINK selection)
    {
        RpcSetSelection(selection);
    }

    [ClientRpc]
    private void RpcSetSelection(PlayerCustomization.DRINK selection)
    {
        coffee.SetActive(selection == PlayerCustomization.DRINK.COFFEE);
        tea.SetActive(selection == PlayerCustomization.DRINK.TEA);
        energyDrink.SetActive(selection == PlayerCustomization.DRINK.ENERGYDRINK);
    }
}
