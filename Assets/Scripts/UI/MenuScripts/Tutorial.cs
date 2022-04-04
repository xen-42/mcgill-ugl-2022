using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialCanvas;
    public GameObject backPageCanvas;

    public void GoToTutorial(){
        tutorialCanvas.SetActive(true);
        if (backPageCanvas.name != "Panel"){
            backPageCanvas.SetActive(false);
        }
    }

    public void GoToPrevCanvas(){
        tutorialCanvas.SetActive(false);
        backPageCanvas.SetActive(true);
    }
}

