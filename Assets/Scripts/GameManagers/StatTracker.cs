using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : NetworkBehaviour
{
    public static StatTracker Instance
    {
        get
        {
            return _instance;
        }
    }
    private static StatTracker _instance = null;

    public PlayerStats clientStats = new PlayerStats { stats = new int[8] };
    public PlayerStats serverStats = new PlayerStats { stats = new int[8] };

    private bool _initialized;

    void Awake()
    {
        if (_instance != null)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            _instance = this;

            _initialized = true;

            EventManager.AddListener("WriteAssignment", OnWriteAssignment);

            if (isClient)
            {
                //netIdentity.AssignClientAuthority(connectionToClient);
            }

            if (isServer)
            {
                RefreshStats();
            }
        }
    }

    private void OnDestroy()
    {
        if (!_initialized) return;

        EventManager.RemoveListener("WriteAssignment", OnWriteAssignment);

        _instance = null;
    }

    public void RefreshStats()
    {
        clientStats = new PlayerStats { stats = new int[8] };
        serverStats = new PlayerStats { stats = new int[8] };
    }

    private void UpdateStat(string name)
    {
        if (isServer)
        {
            // Synced to client automatically
            Debug.Log("Server updating stat");
            serverStats.stats[NameToInt(name)] += 1;
        }
        else
        {
            Debug.Log("Client updating stat");
            CmdUpdateStat(name);
        }
    }

    [Command]
    private void CmdUpdateStat(string name)
    {
        Debug.Log("Hello?");
        clientStats.stats[NameToInt(name)] += 1;
    }

    public void OnWriteAssignment()
    {
        UpdateStat("AssignmentsWritten");
    }

    public void OnScanAssignment()
    {
        UpdateStat("AssignmentsScanned");
    }

    public void OnSubmitAssignment()
    {
        UpdateStat("AssignmentsSubmitted");
    }

    // This is jank but its so it can be a SyncVar
    public struct PlayerStats
    {
        public ulong steamID;
        public string username;

        internal int[] stats;

        public int AssignmentsWritten { get => stats[0]; }
        public int AssignmentsScanned { get => stats[1]; }
        public int AssignmentsSubmitted { get => stats[2]; }
        public int AssignmentsStolen { get => stats[3]; }
        public int DrinksDrank { get => stats[4]; }
        public int CatsPet { get => stats[5]; }
        public int PlantsWatered { get => stats[6]; }
        public int AirConditionersFixed { get => stats[7]; }
    }

    private int NameToInt(string name)
    {
        if (name == "AssignmentsWritten") return 0;
        if (name == "AssignmentsScanned") return 1;
        if (name == "AssignmentsSubmitted") return 2;
        if (name == "AssignmentsStolen") return 3;
        if (name == "DrinksDrank") return 4;
        if (name == "CatsPet") return 5;
        if (name == "PlantsWatered") return 6;
        if (name == "AirConditionersFixed") return 7;

        return -1;
    }
}
