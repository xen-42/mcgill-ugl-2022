using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InputManager;

public class MinigameInteractable : Interactable
{
    [SerializeField] public GameObject MinigamePrefab;

    [SerializeField] public UnityEvent OnCompleteMinigame;

    protected override InputCommand InputCommand { get => InputCommand.Interact; }

    void Start()
    {
        // When the player interacts with this object it'll start the minigame
        _unityEvent.AddListener(() =>
        {
            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(this, MinigamePrefab, out var minigame);
            minigame.OnCompleteMinigame.AddListener(() => {
                OnCompleteMinigame.Invoke();
                IsInteractable = true;
                if(requiredObject != Holdable.Type.NONE && Player.Instance.heldObject != null)
                {
                    Player.Instance.heldObject.Consume();
                }
            });
        });
    }
}
