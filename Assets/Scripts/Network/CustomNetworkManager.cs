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
            return _instance;
        }
    }
    private static CustomNetworkManager _instance;

    [Scene] [SerializeField] private string lobbyMenu = string.Empty;
    [Scene] [SerializeField] private string gameScene = string.Empty;

    [Header("Room")]
    [SerializeField] private LobbyPlayer lobbyPlayerPrefab = null;
    [SerializeField] private Player gamePlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public SteamLobby steamLobby = null;

    public List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();
    public List<Player> players = new List<Player>();

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
    }

    public override void OnStopServer()
    {
        lobbyPlayers.Clear();

        if (SceneManager.GetActiveScene().path == gameScene)
        {
            // Back to main menu
            Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        base.OnStopServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<LobbyPlayer>();

            lobbyPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        OnClientDisconnected?.Invoke();

        if(SceneManager.GetActiveScene().path == gameScene)
        {
            // Back to main menu
            Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

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
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == lobbyMenu)
        {
            bool isLeader = lobbyPlayers.Count == 0;

            LobbyPlayer lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);

            lobbyPlayerInstance.IsLeader = isLeader;

            // Says that this game object represents the new player who connected
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        }
    }

    // Start is called before the first frame update
    new void Awake()
    {
        if(_instance != null)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            _instance = this;

            NetworkClient.RegisterPrefab(lobbyPlayerPrefab.gameObject);
            NetworkClient.RegisterPrefab(gamePlayerPrefab.gameObject);

            _kcpTransport = gameObject.GetComponent<KcpTransport>();
            _steamTransport = gameObject.AddComponent<FizzySteamworks>();
            gameObject.AddComponent<SteamManager>();
            steamLobby = gameObject.AddComponent<SteamLobby>();
        }
    }

    public void Stop()
    {
        // Kick everyone back to the main menu

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
        foreach(var player in lobbyPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    public bool IsReadyToStart()
    {
        // TODO: in release probably make this 2
        if(lobbyPlayers.Count < 1)
        {
            return false;
        }

        foreach(var player in lobbyPlayers)
        {
            if(!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == lobbyMenu)
        {
            if (!IsReadyToStart()) return;

            ServerChangeScene(gameScene);
        }
    }

    public override void ServerChangeScene(string newScene)
    {
        if (SceneManager.GetActiveScene().path == lobbyMenu && newScene == gameScene) 
        {
            for(int i = lobbyPlayers.Count - 1; i >= 0; i--)
            {
                var conn = lobbyPlayers[i].connectionToClient;
                var player = Instantiate(gamePlayerPrefab);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, player.gameObject);
            }
        }

        base.ServerChangeScene(newScene);
    }
}
