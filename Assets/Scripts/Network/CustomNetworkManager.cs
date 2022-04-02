using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
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

    [Header("Lobby")]
    [SerializeField] private LobbyPlayer lobbyPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private Player gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

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
    private LatencySimulation _latencySimulation;

    public const bool ENABLE_LATENCY_SIMULATION = false;

    public void SetTransport(TransportType type)
    {
        switch (type)
        {
            case TransportType.KCP:
                if(ENABLE_LATENCY_SIMULATION)
                {
                    transport = _latencySimulation;
                }
                else
                {
                    transport = _kcpTransport;
                }

                break;
            case TransportType.STEAM:
                transport = _steamTransport;
                break;
        }
        transportType = type;
    }

    public override void OnStartServer()
    {
        Debug.Log("Starting server");
        base.OnStartServer();
    }

    public override void OnStopServer()
    {
        Debug.Log("Stopping server");
        lobbyPlayers.Clear();

        base.OnStopServer();
    }

    public override void OnStartClient()
    {
        RegisterPrefabs();

        Debug.Log("Starting client");
        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        Debug.Log("Stopping client");
        EventManager<string>.TriggerEvent("ConnectionFailed", "Disconnected");
        base.OnStopClient();
    }

    public override void OnClientDisconnect()
    {
        if (SceneManager.GetActiveScene().path == gameScene)
        {
            QuitGame();
        }

        base.OnClientDisconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<LobbyPlayer>();

            lobbyPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        OnClientDisconnected?.Invoke();

        if (SceneManager.GetActiveScene().path == gameScene)
        {
            QuitGame();
        }

        base.OnServerDisconnect(conn);
    }

    public void QuitGame()
    {
        Stop();

        InputManager.CurrentInputMode = InputManager.InputMode.UI;

        // Back to main menu
        if (GameDirector.Instance.GetTimeLeft() > 10)
        {
            SceneManager.LoadScene(Scenes.MainMenu);
        }
        else
        {
            SceneManager.LoadScene(Scenes.GameOver);
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log($"[{conn.identity}] connecting to server");

        base.OnServerConnect(conn);

        // Limit the total number of players
        if (numPlayers >= maxConnections)
        {
            EventManager<string>.TriggerEvent("ConnectionFailed", "Max connections exceeded.");
            conn.Disconnect();
            return;
        }

        // Don't let players join in the middle of a match
        if (SceneManager.GetActiveScene().path != lobbyMenu)
        {
            EventManager<string>.TriggerEvent("ConnectionFailed", "Cannot join on ongoing game.");
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == lobbyMenu)
        {
            bool isLeader = lobbyPlayers.Count == 0;

            Debug.Log("Spawning lobby player");

            LobbyPlayer lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);

            lobbyPlayerInstance.IsLeader = isLeader;

            // Says that this game object represents the new player who connected
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);

            // ID of person that just joined
            lobbyPlayerInstance.SteamID = SteamMatchmaking.GetLobbyMemberByIndex(
                steamLobby.LobbyID,
                numPlayers - 1
            ).m_SteamID;
        }
    }

    // Start is called before the first frame update
    new void Awake()
    {
        Debug.Log($"Starting {nameof(CustomNetworkManager)}");

        RegisterPrefabs();

        if (_instance != null)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            _instance = this;

            _kcpTransport = gameObject.GetComponent<KcpTransport>();
            _steamTransport = gameObject.AddComponent<FizzySteamworks>();
            _latencySimulation = gameObject.GetComponent<LatencySimulation>();
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
        transport.Shutdown();
    }

    private new void OnDestroy()
    {
        Debug.Log($"Destroying {nameof(CustomNetworkManager)}");
        Stop();
        base.OnDestroy();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in lobbyPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    public bool IsReadyToStart()
    {
        // TODO: in release probably make this 2
        if (lobbyPlayers.Count < 1)
        {
            return false;
        }

        foreach (var player in lobbyPlayers)
        {
            if (!player.IsReady)
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
            for (int i = lobbyPlayers.Count - 1; i >= 0; i--)
            {
                Debug.Log("Spawning player");

                var lobbyPlayer = lobbyPlayers[i];

                var conn = lobbyPlayer.connectionToClient;
                var player = Instantiate(gamePlayerPrefab);

                // Disable the audio listener until we get to the game scene
                player.cam.GetComponent<AudioListener>().enabled = false;

                // Pass over the players customization choices
                player.plant = lobbyPlayer.plant;
                player.drink = lobbyPlayer.drink;
                player.poster = lobbyPlayer.poster;

                Debug.Log($"Copied over customization options: {player.plant}, {player.drink}, {player.poster}");

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, player.gameObject);
            }
        }

        base.ServerChangeScene(newScene);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if(sceneName == gameScene)
        {
            Debug.Log("Spawning player spawner");

            //Enable audio listeners for the players
            foreach(var player in players)
            {
                var audioListener = player.cam?.GetComponent<AudioListener>();
                if (audioListener != null)
                {
                    audioListener.enabled = true;
                }
            }

            GameObject playerSpawnerInstance = Instantiate(playerSpawnerPrefab);
            NetworkServer.Spawn(playerSpawnerInstance);
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    private bool _hasRegisteredPrefabs = false;
    private void RegisterPrefabs()
    {
        if (_hasRegisteredPrefabs) return;

        _hasRegisteredPrefabs = true;

        NetworkClient.RegisterPrefab(lobbyPlayerPrefab.gameObject);
        NetworkClient.RegisterPrefab(gamePlayerPrefab.gameObject);
        NetworkClient.RegisterPrefab(playerSpawnerPrefab.gameObject);
    }
}
