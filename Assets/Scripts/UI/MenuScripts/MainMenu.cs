using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        //File -> Build Settings To Access Where The Scenes Are Indexed! 
        //This just goes to the next index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        //Only work when the game is built, force quits the app
        Application.Quit();
    }
}
