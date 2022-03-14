using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MenuScripts
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;

        [Header("Steam")]
        [SerializeField] private TMP_InputField steamLobbyInputField = null;
        [SerializeField] private Button joinSteamButton = null;
        [SerializeField] private Button hostSteamButton = null;

        [Header("LAN")]
        [SerializeField] private Button joinLocalButton = null;
        [SerializeField] private Button hostLocalButton = null;

        private void OnEnable()
        {
            CustomNetworkManager.OnClientConnected += HandleClientConnected;
            CustomNetworkManager.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            CustomNetworkManager.OnClientConnected -= HandleClientConnected;
            CustomNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinSteamLobby()
        {
            string steamLobbyCode = steamLobbyInputField.text;

            CustomNetworkManager.Instance.SetTransport(CustomNetworkManager.TransportType.STEAM);

            if(!CustomNetworkManager.Instance.steamLobby.JoinLobby(steamLobbyCode))
            {
                HandleClientDisconnected();
                return;
            }

            joinLocalButton.interactable = false;
            joinSteamButton.interactable = false;
            hostLocalButton.interactable = false;
            hostSteamButton.interactable = false;
        }

        public void OnSetSteamCode()
        {
            joinSteamButton.interactable = !string.IsNullOrEmpty(steamLobbyInputField.text);
        }

        public void JoinLocalLobby()
        {
            CustomNetworkManager.Instance.SetTransport(CustomNetworkManager.TransportType.KCP);

            CustomNetworkManager.Instance.networkAddress = "localhost";
            CustomNetworkManager.Instance.StartClient();

            joinLocalButton.interactable = false;
            joinSteamButton.interactable = false;
            hostLocalButton.interactable = false;
            hostSteamButton.interactable = false;
        }

        public void HostSteamLobby()
        {
            CustomNetworkManager.Instance.SetTransport(CustomNetworkManager.TransportType.STEAM);

            CustomNetworkManager.Instance.steamLobby.HostLobby();

            joinLocalButton.interactable = false;
            joinSteamButton.interactable = false;
            hostLocalButton.interactable = false;
            hostSteamButton.interactable = false;
        }

        public void HostLocalLobby()
        {
            CustomNetworkManager.Instance.SetTransport(CustomNetworkManager.TransportType.KCP);

            CustomNetworkManager.Instance.networkAddress = "localhost";

            CustomNetworkManager.Instance.StartHost();

            joinLocalButton.interactable = false;
            joinSteamButton.interactable = false;
            hostLocalButton.interactable = false;
            hostSteamButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            // This is so that if they quit out back to the lobby these buttons are re-enabled
            joinLocalButton.interactable = true;
            joinSteamButton.interactable = true;
            hostLocalButton.interactable = true;
            hostSteamButton.interactable = true;

            landingPagePanel.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            // This is so that if they fail to connect and go back to the lobby these buttons are re-enabled
            joinLocalButton.interactable = true;
            joinSteamButton.interactable = true;
            hostLocalButton.interactable = true;
            hostSteamButton.interactable = true;

            landingPagePanel.SetActive(true);
        }
    }
}
