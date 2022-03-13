using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InputManager;

public class ComputerInteractable : Interactable
{
    [SerializeField]
    public GameObject MinigamePrefab;

    protected override InputCommand InputCommand { get => InputCommand.Interact; }

    void Start()
    {
        // When the player interacts with this object it'll start the minigame
        _unityEvent.AddListener(() =>
        {
            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(MinigamePrefab, out var minigame);

            // Tell the minigame to run this method when the player finishes
            minigame.OnCompleteMinigame.AddListener(OnCompleteMinigame);
        });
    }

    protected override bool HasItem()
    {
        // Instead of checking for a held item we check the game director for the condition
        return (GameDirector.Instance.NumAssignmentsScanned > GameDirector.Instance.NumAssignmentsDone);
    }

    private void OnCompleteMinigame()
    {
        IsInteractable = true;
        GameDirector.Instance.DoAssignment();
    }
}
