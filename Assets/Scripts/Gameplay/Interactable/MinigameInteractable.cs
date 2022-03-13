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
        _unityEvent.AddListener(() =>
        {
            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(MinigamePrefab, out var minigame);
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
