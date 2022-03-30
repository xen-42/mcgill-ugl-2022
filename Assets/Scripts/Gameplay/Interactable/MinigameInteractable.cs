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
            minigame.OnCompleteMinigame.AddListener(() => OnCompleteMinigame.Invoke());
            minigame.OnCompleteMinigame.AddListener(() => IsInteractable = true);
            if (requiredObject != Holdable.Type.NONE)
            {
                // Should never be null but could be a bug and it'll hang the player
                if (Player.Instance.heldObject != null)
                {
                    // Try consuming the required object after use
                    minigame.OnCompleteMinigame.AddListener(() => Player.Instance.heldObject.Consume());
                }
            }
        });
    }
}
