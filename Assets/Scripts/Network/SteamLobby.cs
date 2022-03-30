using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CustomNetworkManager))]
public class SteamLobby : MonoBehaviour
{
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAddressKey = "HostAddress";

    private CustomNetworkManager _networkManager;

    public CSteamID LobbyID { get; private set; }

    private void Start()
    {
        _networkManager = GetComponent<CustomNetworkManager>();

        if (!SteamManager.Initialized) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
    }

    public bool JoinLobby(string code)
    {
        try
        {
            Debug.Log($"Trying to join {Convert.ToUInt64(code)}");
            SteamMatchmaking.JoinLobby(new CSteamID(Convert.ToUInt64(code)));
            return true;
        }
        catch (Exception e)
        {
            EventManager<string>.TriggerEvent("ConnectionFailed", e.Message);
            Debug.LogError(e.Message);
            return false;
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            var errorMsg = $"Failed to create lobby [{callback.m_ulSteamIDLobby}] reason: [{callback.m_eResult}]";
            EventManager<string>.TriggerEvent("ConnectionFailed", errorMsg);
            Debug.Log(errorMsg);
            return;
        }

        _networkManager.StartHost();

        var lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(lobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(lobbyID, "name", SteamFriends.GetPersonaName().ToString());

        LobbyID = lobbyID;

        Debug.Log($"Created lobby {lobbyID.m_SteamID}");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log($"Request to join lobby [{callback.m_steamIDLobby}]");

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        Debug.Log($"Joining lobby [{callback.m_ulSteamIDLobby}]");

        // We're a client trying to join
        if (!NetworkServer.active)
        {
            _networkManager.networkAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey
            );

            if(string.IsNullOrWhiteSpace(_networkManager.networkAddress))
            {
                var errorMsg = $"Couldn't connect to [{callback.m_ulSteamIDLobby}]";
                Debug.Log(errorMsg);
                EventManager<string>.TriggerEvent("ConnectionFailed", errorMsg);
                return;
            }

            _networkManager.StartClient();
        }
    }
}
