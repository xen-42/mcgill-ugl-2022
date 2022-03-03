using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Minigame : MonoBehaviour
{
    [SerializeField]
    public MinigameEvent onCompleteMinigame;

    public void CompleteMinigame()
    {
        EventManager.TriggerEvent("MinigameComplete");
        onCompleteMinigame.Invoke();
        Debug.Log("MinigameComplete");

        // Once a minigame is completed we just dispose of it
        Destroy(gameObject);
    }

    [System.Serializable]
    public class MinigameEvent : UnityEvent { }
}
