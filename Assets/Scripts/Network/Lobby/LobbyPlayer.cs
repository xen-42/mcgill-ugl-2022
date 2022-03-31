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

    private Sprite steamAvatarSprite = null;
    [SyncVar] private int Ping = 0;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))] public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))] public bool IsReady = false;
    [SyncVar(hook = nameof(HandleSteamIDChanged))] public ulong SteamID;

    protected Callback<AvatarImageLoaded_t> _avatarImageLoaded;

    private bool _connected = false;

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
    }

    [SyncVar] private bool isLeader;
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
        var cSteamID = new CSteamID(newValue);
        LoadAvatar(SteamFriends.GetLargeFriendAvatar(cSteamID));
    }

    public void Update()
    {
        if (!_connected) return;

        if(hasAuthority)
        {
            SetPing((int)Math.Round(NetworkTime.rtt * 1000));
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < CustomNetworkManager.Instance.lobbyPlayers.Count)
            {
                var player = CustomNetworkManager.Instance.lobbyPlayers[i];

                var displayName = player.DisplayName.Length < 12 ? player.DisplayName : player.DisplayName.Substring(0, 11) + "...";
                
                if(player.isLeader)
                {
                    playerNameTexts[i].text = $"{displayName}\n";
                }
                else
                {
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
                if(player.steamAvatarSprite != null)
                {
                    playerAvatars[i].sprite = steamAvatarSprite;
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
        if (isLeader)
        {
            CustomNetworkManager.Instance.StopHost();
        }
        else
        {
            CustomNetworkManager.Instance.StopClient();
        }
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
        if (callback.m_steamID.m_SteamID == SteamID)
        {
            Debug.Log("Updating avatar from callback");
            LoadAvatar(callback.m_iImage);
        }
    }

    private bool LoadAvatar(int imageID)
    {
        Debug.Log($"Loading [{imageID}] for [{DisplayName}]");

        if (imageID != -1)
        {
            if (SteamUtils.GetImageSize(imageID, out uint width, out uint height))
            {
                var size = width * height * 4;
                byte[] image = new byte[size];
                if (SteamUtils.GetImageRGBA(imageID, image, (int)size))
                {
                    var texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                    texture.LoadRawTextureData(image);
                    texture.Apply();
                    steamAvatarSprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(texture.width / 2f, texture.height / 2f)
                        );

                    UpdateDisplay();

                    return true;
                }
            }
        }
        
        return false;
    }

    private void SetPing(int ping)
    {
        if(isServer)
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
}

