using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("NetworkManager");
                _instance = obj.AddComponent<CustomNetworkManager>();
            }
            return _instance;
        }
        set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else
            {
                Destroy(value.gameObject);
            }
        }
    }
    private static CustomNetworkManager _instance;

    [Scene] [SerializeField] private string lobbyMenu = string.Empty;

    [Header("Room")]
    [SerializeField] private StudentXPLobbyPlayer roomPlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public SteamLobby steamLobby = null;

    public List<StudentXPLobbyPlayer> players = new List<StudentXPLobbyPlayer>();

    public enum TransportType
    {
        KCP,
        STEAM
    }
    public TransportType transportType;

    private KcpTransport _kcpTransport;
    private FizzySteamworks _steamTransport;

    public void SetTransport(TransportType type)
    {
        switch (type)
        {
            case TransportType.KCP:
                transport = _kcpTransport;
                break;
            case TransportType.STEAM:
                transport = _steamTransport;
                break;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Resources.LoadAll<GameObject>("Prefabs").ToList();
    }

    public override void OnStopServer()
    {
        players.Clear();

        base.OnStopServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        var spawnablePrefabs = Resources.LoadAll<GameObject>("Prefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<StudentXPLobbyPlayer>();

            players.Remove(player);

            NotifyPlayersOfReadyState();
        }

        OnClientDisconnected?.Invoke();

        base.OnServerDisconnect(conn);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        // Limit the total number of players
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        // Don't let players join in the middle of a match
        if (SceneManager.GetActiveScene().path != lobbyMenu)
        {
            conn.Disconnect();
            return;
        }

        if (conn?.identity?.GetComponent<StudentXPLobbyPlayer>() != null)
        {
            players.Add(conn.identity.GetComponent<StudentXPLobbyPlayer>());
        }
        else
        {
            Debug.LogError($"Couldn't find StudentXPLobbyPlayer for {conn}");
        }

    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == lobbyMenu)
        {
            bool isLeader = players.Count == 0;

            StudentXPLobbyPlayer lobbyPlayerInstance = Instantiate(roomPlayerPrefab);

            lobbyPlayerInstance.IsLeader = isLeader;

            // Says that this game object represents the new player who connected
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        }
    }

    // Start is called before the first frame update
    new void Awake()
    {
        Instance = this;

        _kcpTransport = gameObject.GetComponent<KcpTransport>();
        _steamTransport = gameObject.AddComponent<FizzySteamworks>();
        gameObject.AddComponent<SteamManager>();
        steamLobby = gameObject.AddComponent<SteamLobby>();
    }

    public void Stop()
    {
        if (NetworkClient.isHostClient)
        {
            StopHost();
        }
        else
        {
            StopClient();
        }
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        Stop();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach(var player in players)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    public bool IsReadyToStart()
    {
        if(players.Count < 2)
        {
            return false;
        }

        foreach(var player in players)
        {
            if(!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }
}
