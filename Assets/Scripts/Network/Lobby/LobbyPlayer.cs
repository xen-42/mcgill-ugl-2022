using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[2];
    [SerializeField] private Button startGameButton = null;

    [SerializeField] private TMP_Text steamLobbyCode = null;
    [SerializeField] private Button copySteamCodeButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))] public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))] public bool IsReady = false;

    private void Awake()
    {
        lobbyUI.SetActive(false);

        if (CustomNetworkManager.Instance.transportType == CustomNetworkManager.TransportType.STEAM)
        {
            steamLobbyCode.SetText(CustomNetworkManager.Instance.steamLobby.LobbyID);

            copySteamCodeButton.interactable = true;
        }
        else
        {
            steamLobbyCode.gameObject.SetActive(false);
            copySteamCodeButton.gameObject.SetActive(false);
        }
    }

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    public void CopySteamCode()
    {
        GUIUtility.systemCopyBuffer = steamLobbyCode.text;
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        CustomNetworkManager.Instance.lobbyPlayers.Add(this);

        UpdateDisplay();

        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        CustomNetworkManager.Instance.lobbyPlayers.Remove(this);

        UpdateDisplay();

        base.OnStopClient();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    public void UpdateDisplay()
    {
        if(!hasAuthority)
        {
            foreach(var player in CustomNetworkManager.Instance.lobbyPlayers)
            {
                if(player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        var playerCount = CustomNetworkManager.Instance.lobbyPlayers.Count;

        for(int i = 0; i < playerNameTexts.Length; i++)
        {
            if(i < playerCount)
            {
                var player = CustomNetworkManager.Instance.lobbyPlayers[i];
                playerNameTexts[i].text = player.DisplayName;
                playerReadyTexts[i].text = player.IsReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
            }
            else
            {
                playerNameTexts[i].text = "Waiting for player...";
                playerReadyTexts[i].text = string.Empty;
            }
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if(!isLeader)
        {
            return;
        }

        startGameButton.interactable = readyToStart;
    }

    #region Commands

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        CustomNetworkManager.Instance.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        // Verify that its the first person in the list i.e. the leader
        if(CustomNetworkManager.Instance.lobbyPlayers[0].connectionToClient != connectionToClient)
        {
            return;
        }

        CustomNetworkManager.Instance.StartGame();
    }

    #endregion Commands
}

