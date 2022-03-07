using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinigameInteractable : Interactable
{
    [SerializeField]
    public GameObject MinigamePrefab;

    [SerializeField]
    public UnityEvent OnCompleteMinigame;

    void Start()
    {
        // When the player interacts with this object it'll start the minigame
        _event.AddListener(() => {
            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(MinigamePrefab, out var minigame);
            minigame.OnCompleteMinigame.AddListener(() => OnCompleteMinigame.Invoke());
        });
    }
}
