using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posters : NetworkBehaviour
{
    [SerializeField] private GameObject[] Set1 = new GameObject[3];
    [SerializeField] private GameObject[] Set2 = new GameObject[3];
    [SerializeField] private GameObject[] Set3 = new GameObject[3];

    private bool _initialized = false;
    [SyncVar] private PlayerCustomization.POSTER posterSelection;

    private void Start()
    {
        // At this point only the client should be un-initialized
        if (!_initialized)
        {
            ShowPosters();
        }
    }

    [Server]
    public void SetSelection(PlayerCustomization.POSTER selection)
    {
        posterSelection = selection;

        ShowPosters();
    }

    private void ShowPosters()
    {
        _initialized = true;

        foreach (var poster in Set1)
        {
            poster.SetActive(posterSelection == PlayerCustomization.POSTER.SET1);
        }
        foreach (var poster in Set2)
        {
            poster.SetActive(posterSelection == PlayerCustomization.POSTER.SET2);
        }
        foreach (var poster in Set3)
        {
            poster.SetActive(posterSelection == PlayerCustomization.POSTER.SET3);
        }
    }
}
