using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Minigame : MonoBehaviour
{
    [SerializeField]
    public MinigameEvent onCompleteMinigame;

    public void Start()
    {
        // Don't want to be walking around while in a minigame
        InputManager.CurrentInputMode = InputManager.InputMode.Minigame;
        Debug.Log("Minigame starting");
    }

    public void CompleteMinigame()
    {
        EventManager.TriggerEvent("MinigameComplete");
        onCompleteMinigame.Invoke();
        Debug.Log("MinigameComplete");
        InputManager.CurrentInputMode = InputManager.InputMode.Player;

        // Once a minigame is completed we just dispose of it
        Destroy(gameObject);
    }

    [System.Serializable]
    public class MinigameEvent : UnityEvent { }
}
