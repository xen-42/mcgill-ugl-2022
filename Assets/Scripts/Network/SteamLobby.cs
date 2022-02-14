using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CustomNetworkManager))]
public class SteamLobby : MonoBehaviour
{
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAddressKey = "HostAddress";

    private CustomNetworkManager _networkManager;

    [SerializeField]
    private InputField steamIDField;

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
        _networkManager.SetSteamNetworkUIVisibility(false);

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
    }

    public void JoinLobby()
    {
        try
        {
            Debug.Log($"Trying to join {Convert.ToUInt64(steamIDField.text)}");
            SteamMatchmaking.JoinLobby(new CSteamID(Convert.ToUInt64(steamIDField.text)));
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            _networkManager.SetSteamNetworkUIVisibility(true);
            return;
        }

        _networkManager.StartHost();

        var lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(lobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(lobbyID, "name", SteamFriends.GetPersonaName().ToString());

        GUIUtility.systemCopyBuffer = lobbyID.m_SteamID.ToString();
        Debug.Log($"Created lobby {lobbyID.m_SteamID}");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join lobby");

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);       
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        Debug.Log($"Joining lobby {callback.m_ulSteamIDLobby}");

        // We're a client trying to join
        if (!NetworkServer.active)
        {
            _networkManager.networkAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey
            );

            _networkManager.StartClient();

            _networkManager.SetSteamNetworkUIVisibility(false);
        }
    }
}
