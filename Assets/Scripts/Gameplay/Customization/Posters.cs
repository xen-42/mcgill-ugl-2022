using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posters : NetworkBehaviour
{
    [SerializeField] private GameObject[] Set1 = new GameObject[3];
    [SerializeField] private GameObject[] Set2 = new GameObject[3];
    [SerializeField] private GameObject[] Set3 = new GameObject[3];

    [Server]
    public void SetSelection(PlayerCustomization.POSTER selection)
    {
        RpcSetSelection(selection);
    }

    [ClientRpc]
    private void RpcSetSelection(PlayerCustomization.POSTER selection)
    {
        foreach (var poster in Set1)
        {
            poster.SetActive(selection == PlayerCustomization.POSTER.SET1);
        }
        foreach (var poster in Set2)
        {
            poster.SetActive(selection == PlayerCustomization.POSTER.SET2);
        }
        foreach (var poster in Set3)
        {
            poster.SetActive(selection == PlayerCustomization.POSTER.SET3);
        }
    }
}
