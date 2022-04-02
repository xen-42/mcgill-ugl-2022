using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        InputManager.CurrentInputMode = InputManager.InputMode.UI;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(Scenes.Lobby);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        //Only work when the game is built, force quits the app
        Application.Quit();
    }

    public void ShowCredits(){
        SceneManager.LoadScene(Scenes.Credits);
    }
}
