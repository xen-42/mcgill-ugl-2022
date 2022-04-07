using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MenuScripts
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;
        [SerializeField] private Text connectingText = null;

        [Header("PopUp")]
        [SerializeField] private GameObject popUp = null;
        [SerializeField] private Text errorText = null;

        [Header("Steam")]
        [SerializeField] private TMP_InputField steamLobbyInputField = null;
        [SerializeField] private Button joinSteamButton = null;
        [SerializeField] private Button hostSteamButton = null;

        [Header("LAN")]
        [SerializeField] private Button joinLocalButton = null;
        [SerializeField] private Button hostLocalButton = null;

        private void Awake()
        {
            joinSteamButton.interactable = !string.IsNullOrEmpty(steamLobbyInputField.text);
        }

        private void OnEnable()
        {
            CustomNetworkManager.OnClientConnected += HandleClientConnected;
            CustomNetworkManager.OnClientDisconnected += HandleClientDisconnected;

            connectingText.enabled = false;

            EventManager<string>.AddListener("ConnectionFailed", OnConnectionFailed);
        }

        private void OnDisable()
        {
            CustomNetworkManager.OnClientConnected -= HandleClientConnected;
            CustomNetworkManager.OnClientDisconnected -= HandleClientDisconnected;

            EventManager<string>.RemoveListener("ConnectionFailed", OnConnectionFailed);
        }

        public void JoinSteamLobby()
        {
            if (!SteamManager.Initialized)
            {
                OnConnectionFailed("Can't find Steam, did you add the game to your steam library?");
                return;
            }

            string steamLobbyCode = steamLobbyInputField.text;

            CustomNetworkManager.Instance.SetTransport(CustomNetworkManager.TransportType.STEAM);

            if (!CustomNetworkManager.Instance.steamLobby.JoinLobby(steamLobbyCode))
            {
                HandleClientDisconnected();
                return;
            }

            joinLocalButton.interactable = false;
            joinSteamButton.interactable = false;
            hostLocalButton.interactable = false;
            hostSteamButton.interactable = false;

            connectingText.enabled = true;
        }

        public void OnSetSteamCode()
        {
            joinSteamButton.interactable = !string.IsNullOrEmpty(steamLobbyInputField.text);
        }

        public void JoinLocalLobby()
        {
            if (!SteamManager.Initialized)
            {
                OnConnectionFailed("Can't find Steam, did you add the game to your steam library?");
                return;
            }

            CustomNetworkManager.Instance.SetTransport(CustomNetworkManager.TransportType.KCP);

            CustomNetworkManager.Instance.networkAddress = "localhost";
            CustomNetworkManager.Instance.StartClient();

            joinLocalButton.interactable = false;
            joinSteamButton.interactable = false;
            hostLocalButton.interactable = false;
            hostSteamButton.interactable = false;

            connectingText.enabled = true;
        }

        public void HostSteamLobby()
        {
            if (!SteamManager.Initialized)
            {
                OnConnectionFailed("Can't find Steam, did you add the game to your steam library?");
                return;
            }

            CustomNetworkManager.Instance.SetTransport(CustomNetworkManager.TransportType.STEAM);

            CustomNetworkManager.Instance.steamLobby.HostLobby();

            joinLocalButton.interactable = false;
            joinSteamButton.interactable = false;
            hostLocalButton.interactable = false;
            hostSteamButton.interactable = false;

            connectingText.enabled = true;
        }

        public void HostLocalLobby()
        {
            if (!SteamManager.Initialized)
            {
                OnConnectionFailed("Can't find Steam, did you add the game to your steam library?");
                return;
            }

            CustomNetworkManager.Instance.SetTransport(CustomNetworkManager.TransportType.KCP);

            CustomNetworkManager.Instance.networkAddress = "localhost";

            CustomNetworkManager.Instance.StartHost();

            joinLocalButton.interactable = false;
            joinSteamButton.interactable = false;
            hostLocalButton.interactable = false;
            hostSteamButton.interactable = false;

            connectingText.enabled = true;
        }

        private void HandleClientConnected()
        {
            // This is so that if they quit out back to the lobby these buttons are re-enabled
            joinLocalButton.interactable = true;
            joinSteamButton.interactable = !string.IsNullOrEmpty(steamLobbyInputField.text);
            hostLocalButton.interactable = true;
            hostSteamButton.interactable = true;

            landingPagePanel.SetActive(false);

            connectingText.enabled = false;
        }

        private void HandleClientDisconnected()
        {
            OnConnectionFailed($"Connection failed.");
        }

        private void OnConnectionFailed(string errorMessage)
        {
            popUp.SetActive(true);
            errorText.text = errorMessage;

            // Just making sure it re-enables the UI
            joinLocalButton.interactable = true;
            joinSteamButton.interactable = !string.IsNullOrEmpty(steamLobbyInputField.text);
            hostLocalButton.interactable = true;
            hostSteamButton.interactable = true;

            landingPagePanel.SetActive(true);

            connectingText.enabled = false;
        }

        public void OnBackButtonPressed()
        {
            // Just reload the scene
            SceneManager.LoadScene(Scenes.Lobby);
        }

    }
}
