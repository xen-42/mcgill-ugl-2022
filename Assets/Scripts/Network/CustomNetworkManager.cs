using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager Instance;

    public enum TransportType
    {
        KCP,
        STEAM
    }

    public TransportType transportType;

    public GameObject steamNetworkUI;

    // Start is called before the first frame update
    new void Awake()
    {
        Instance = this;

        switch (transportType)
        {
            case (TransportType.KCP):
                transport = gameObject.AddComponent<KcpTransport>();
                gameObject.AddComponent<NetworkManagerHUD>();
                SetSteamNetworkUIVisibility(false);
                break;
            case (TransportType.STEAM):
                transport = gameObject.AddComponent<FizzySteamworks>();
                gameObject.AddComponent<SteamManager>();
                SetSteamNetworkUIVisibility(true);
                break;
        }
    }

    public void SetSteamNetworkUIVisibility(bool isVisible)
    {
        steamNetworkUI.SetActive(isVisible);
    }
}
