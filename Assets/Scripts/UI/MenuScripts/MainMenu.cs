using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Scene] [SerializeField] private string lobbyMenu = null;

    private void Start()
    {
        InputManager.CurrentInputMode = InputManager.InputMode.UI;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(lobbyMenu);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        //Only work when the game is built, force quits the app
        Application.Quit();
    }
}
