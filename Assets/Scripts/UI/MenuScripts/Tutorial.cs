using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialUI;

    public void BacktoMainMenu(){
        SceneManager.LoadScene(Scenes.Lobby);
    }

    public void BacktoGame(){
        tutorialUI.SetActive(false);
        InputManager.CurrentInputMode = InputManager.InputMode.Player;
    }
}
