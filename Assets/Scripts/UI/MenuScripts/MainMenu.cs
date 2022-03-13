using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Scene] [SerializeField] private string lobbyMenu = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        CustomNetworkManager.Instance.StartHost();

        landingPagePanel.SetActive(false);
    }

    public void PlayGame()
    {
        //File -> Build Settings To Access Where The Scenes Are Indexed! 
        //This just goes to the next index
        SceneManager.LoadScene(lobbyMenu);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        //Only work when the game is built, force quits the app
        Application.Quit();
    }
}
