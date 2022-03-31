using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public void BacktoMainMenu(){
        SceneManager.LoadScene(Scenes.MainMenu);
    }
}
