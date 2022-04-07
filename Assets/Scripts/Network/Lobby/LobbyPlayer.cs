using Mirror;
using Steamworks;
using System;
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
    [SerializeField] private Image[] playerAvatars = new Image[2];

    [SerializeField] private Button startGameButton = null;

    [SerializeField] private TMP_Text steamLobbyCode = null;
    [SerializeField] private Button copySteamCodeButton = null;

    [SyncVar] private int Ping = 0;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))] public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))] public bool IsReady = false;
    [SyncVar(hook = nameof(HandleSteamIDChanged))] public ulong SteamID;

    [Header("Customization")]
    [SerializeField] UISelector plantSelection;
    [SerializeField] UISelector drinkSelection;
    [SerializeField] UISelector posterSelection;

    [SyncVar] public PlayerCustomization.PLANT plant;
    [SyncVar] public PlayerCustomization.DRINK drink;
    [SyncVar] public PlayerCustomization.POSTER poster;
    [SyncVar] public PlayerCustomization.COLOUR colour;
    [SyncVar] public bool hasChosenCharacter = false;

    [SerializeField] GameObject warmCharacterButton;
    [SerializeField] GameObject coolCharacterButton;

    protected Callback<AvatarImageLoaded_t> _avatarImageLoaded;

    private bool _connected = false;

    private Image _coolCheck;
    private Image _warmCheck;
    private Image _coolCross;
    private Image _warmCross;
    private Button _coolButton;
    private Button _warmButton;

    private void Awake()
    {
        InputManager.CurrentInputMode = InputManager.InputMode.UI;

        lobbyUI.SetActive(false);

        if (CustomNetworkManager.Instance.transportType == CustomNetworkManager.TransportType.STEAM)
        {
            Debug.Log("Steam transport");

            steamLobbyCode.SetText(CustomNetworkManager.Instance.steamLobby.LobbyID.ToString());
            copySteamCodeButton.interactable = true;

            steamLobbyCode.gameObject.SetActive(true);
            copySteamCodeButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Kcp transport");

            steamLobbyCode.gameObject.SetActive(false);
            copySteamCodeButton.gameObject.SetActive(false);
        }

        UpdateDisplay();

        plantSelection.OnSetSelection.AddListener(OnSetPlantSelection);
        drinkSelection.OnSetSelection.AddListener(OnSetDrinkSelection);
        posterSelection.OnSetSelection.AddListener(OnSetPosterSelection);

        startGameButton.gameObject.SetActive(isLeader);

        _coolCheck = coolCharacterButton.transform.Find("Check").GetComponent<Image>();
        _warmCheck = warmCharacterButton.transform.Find("Check").GetComponent<Image>();
        _coolCross = coolCharacterButton.transform.Find("Cross").GetComponent<Image>();
        _warmCross = warmCharacterButton.transform.Find("Cross").GetComponent<Image>();
        _coolButton = coolCharacterButton.GetComponent<Button>();
        _warmButton = warmCharacterButton.GetComponent<Button>();

        // check for if the other player has already chosen somebody
        foreach (var player in CustomNetworkManager.Instance.lobbyPlayers)
        {
            if (player.netId != netId)
            {
                if(player.hasChosenCharacter)
                {
                    if(player.colour == PlayerCustomization.COLOUR.WARM)
                    {
                        _warmCross.enabled = true;
                        _warmButton.enabled = false;
                    }
                    else
                    {
                        _coolCross.enabled = true;
                        _coolButton.enabled = false;
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        plantSelection.OnSetSelection.RemoveListener(OnSetPlantSelection);
        drinkSelection.OnSetSelection.RemoveListener(OnSetDrinkSelection);
        posterSelection.OnSetSelection.RemoveListener(OnSetPosterSelection);

        if(hasChosenCharacter)
        {
            var cool = colour == PlayerCustomization.COLOUR.COOL;
            foreach (var player in CustomNetworkManager.Instance.lobbyPlayers)
            {
                if (player.netId == netId) continue;
                player.OnPlayerDisconnect(cool);
            }
        }
    }

    [SyncVar] private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
            colour = value ? PlayerCustomization.COLOUR.COOL : PlayerCustomization.COLOUR.WARM;
        }
    }

    public void CopySteamCode()
    {
        GUIUtility.systemCopyBuffer = steamLobbyCode.text;
    }

    public void OnPlayerDisconnect(bool cool)
    {
        Debug.Log($"Player disconnected who had chosen {(cool ? "cool" : "warm")}");

        if(isServer)
        {
            RpcOnPlayerDisconnect(cool);
        }
        else
        {
            CmdOnPlayerDisconnect(cool);
        }
    }

    [Command]
    public void CmdOnPlayerDisconnect(bool cool)
    {
        RpcOnPlayerDisconnect(cool);
    }

    [ClientRpc]
    public void RpcOnPlayerDisconnect(bool cool)
    {
        if(cool)
        {
            _coolButton.enabled = true;
            _coolCross.enabled = false;
        }
        else
        {
            _warmButton.enabled = true;
            _warmCross.enabled = false;
        }
    }

    public void SelectCharacter(bool cool)
    {
        if (isServer)
        {
            ServerSelectCharacter(cool, netId);
        }
        else
        {
            CmdSelectCharacter(cool, netId);
        }
    }

    [Command]
    public void CmdSelectCharacter(bool cool, uint id)
    {
        ServerSelectCharacter(cool, id);
    }

    [Server]
    public void ServerSelectCharacter(bool cool, uint id)
    {
        var chosenColour = cool ? PlayerCustomization.COLOUR.COOL : PlayerCustomization.COLOUR.WARM;
        var otherColour = !cool ? PlayerCustomization.COLOUR.COOL : PlayerCustomization.COLOUR.WARM;
        LobbyPlayer activePlayer = null;
        LobbyPlayer otherPlayer = null;

        foreach (var player in CustomNetworkManager.Instance.lobbyPlayers)
        {
            if (player.netId == id)
            {
                activePlayer = player;
            }
            else
            {
                otherPlayer = player;
            }
        }

        // This shouldn't happen but just to be safe
        if (activePlayer == null) return;

        if (activePlayer.hasChosenCharacter && activePlayer.colour == chosenColour)
        {
            // We're deselecting the current choice
            activePlayer.hasChosenCharacter = false;
            activePlayer.RpcSelectCharacter(cool, id, false);
            if(otherPlayer)
            {
                otherPlayer.RpcSelectCharacter(cool, id, false);
            }
        }
        else
        {
            // Can they be this character
            if (otherPlayer == null || !(otherPlayer.colour == chosenColour && otherPlayer.hasChosenCharacter))
            {
                // Will automatically be synced
                activePlayer.colour = chosenColour;
                activePlayer.hasChosenCharacter = true;

                // Make the other player take the other colour but not lock it in
                if (otherPlayer) otherPlayer.colour = otherColour;

                activePlayer.RpcSelectCharacter(cool, id, true);

                if (otherPlayer)
                {
                    otherPlayer.colour = !cool ? PlayerCustomization.COLOUR.COOL : PlayerCustomization.COLOUR.WARM;
                    otherPlayer.RpcSelectCharacter(cool, id, true);
                }
            }
        }
    }

    [ClientRpc]
    public void RpcSelectCharacter(bool cool, uint id, bool chosen)
    {
        Debug.Log($"[{id}] {(chosen ? "selected" : "deselected")} [{(cool ? "cool" : "warm")}]");

        if (netId == id)
        {
            if (chosen)
            {
                _coolCheck.enabled = cool;
                _warmCheck.enabled = !cool;
            }
            else
            {
                _coolCheck.enabled = false;
                _warmCheck.enabled = false;
            }
        }
        else
        {
            if (chosen)
            {
                // Have to stop the other person
                _coolButton.enabled = !cool;
                _warmButton.enabled = cool;

                _coolCross.enabled = cool;
                _warmCross.enabled = !cool;
            }
            else
            {
                if (cool)
                {
                    _coolCross.enabled = false;
                    _coolButton.enabled = true;
                }
                else
                {
                    _warmCross.enabled = false;
                    _warmButton.enabled = true;
                }
            }
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);

        // If we have authority then this is us
        CmdSetSteamID(SteamUser.GetSteamID().m_SteamID);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        _connected = true;

        _avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);

        CustomNetworkManager.Instance.lobbyPlayers.Add(this);

        UpdateDisplay();

        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        _connected = false;

        CustomNetworkManager.Instance.lobbyPlayers.Remove(this);

        UpdateDisplay();

        base.OnStopClient();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    public void HandleSteamIDChanged(ulong oldValue, ulong newValue)
    {
        Debug.Log($"Steam ID was changed to [{newValue}] for [{DisplayName}]");
        SteamID = newValue;
        UpdateDisplay();
    }

    public void Update()
    {
        if (!_connected) return;

        if (hasAuthority)
        {
            SetPing((int)Math.Round(NetworkTime.rtt * 1000));
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < CustomNetworkManager.Instance.lobbyPlayers.Count)
            {
                var player = CustomNetworkManager.Instance.lobbyPlayers[i];

                var displayName = player.DisplayName.Length < 12 ? player.DisplayName : player.DisplayName.Substring(0, 11) + "...";

                if (player.isLeader)
                {
                    StatTracker.serverUserName = displayName;
                    StatTracker.serverSteamID = player.SteamID;
                    playerNameTexts[i].text = $"{displayName}\n";
                }
                else
                {
                    StatTracker.clientUserName = displayName;
                    StatTracker.clientSteamID = player.SteamID;
                    playerNameTexts[i].text = $"{displayName}\n{player.Ping}ms";
                }
            }
        }
    }

    public void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in CustomNetworkManager.Instance.lobbyPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        var playerCount = CustomNetworkManager.Instance.lobbyPlayers.Count;

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < playerCount)
            {
                var player = CustomNetworkManager.Instance.lobbyPlayers[i];
                playerNameTexts[i].text = $"{player.DisplayName}";
                playerReadyTexts[i].text = player.IsReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";

                var sprite = Utils.LoadAvatar(SteamFriends.GetLargeFriendAvatar(new CSteamID(player.SteamID)));

                if (sprite != null)
                {
                    playerAvatars[i].sprite = sprite;
                    playerAvatars[i].gameObject.SetActive(true);
                }
                else
                {
                    playerAvatars[i].gameObject.SetActive(false);
                }
            }
            else
            {
                playerNameTexts[i].text = "Waiting for player...";
                playerReadyTexts[i].text = string.Empty;
                playerAvatars[i].gameObject.SetActive(false);
            }
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader)
        {
            return;
        }

        startGameButton.interactable = readyToStart;
    }

    public void OnBackButtonPressed()
    {
        CustomNetworkManager.Instance.Stop();
        EventManager<string>.TriggerEvent("ConnectionFailed", "Disconnected");
        Destroy(gameObject);
    }

    #region Commands

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    private void CmdSetSteamID(ulong steamID)
    {
        SteamID = steamID;
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
        if (CustomNetworkManager.Instance.lobbyPlayers[0].connectionToClient != connectionToClient)
        {
            return;
        }

        CustomNetworkManager.Instance.StartGame();
    }

    #endregion Commands

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        UpdateDisplay();
    }

    private void SetPing(int ping)
    {
        if (isServer)
        {
            Ping = ping;
        }
        else
        {
            CmdSetPing(ping);
        }
    }

    [Command]
    private void CmdSetPing(int ping)
    {
        Ping = ping;
    }

    #region Customization Stuff

    public void OnSetPlantSelection(int selection)
    {
        if (isServer)
        {
            plant = (PlayerCustomization.PLANT)selection;
        }
        else
        {
            CmdSetPlantSelection(selection);
        }
    }

    [Command]
    public void CmdSetPlantSelection(int selection)
    {
        plant = (PlayerCustomization.PLANT)selection;
    }

    public void OnSetDrinkSelection(int selection)
    {
        if (isServer)
        {
            drink = (PlayerCustomization.DRINK)selection;
        }
        else
        {
            CmdSetDrinkSelection(selection);
        }
    }

    [Command]
    public void CmdSetDrinkSelection(int selection)
    {
        drink = (PlayerCustomization.DRINK)selection;
    }

    public void OnSetPosterSelection(int selection)
    {
        if (isServer)
        {
            poster = (PlayerCustomization.POSTER)selection;
        }
        else
        {
            CmdSetPosterSelection(selection);
        }
    }

    [Command]
    public void CmdSetPosterSelection(int selection)
    {
        poster = (PlayerCustomization.POSTER)selection;
    }

    #endregion Customization Stuff
}

