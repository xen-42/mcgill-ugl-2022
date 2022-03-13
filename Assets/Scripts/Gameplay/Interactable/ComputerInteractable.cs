using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComputerInteractable : Interactable
{
    public GameObject minigamePrefab;

    private void Start()
    {
        // When the player interacts with this object it'll start the minigame
        _unityEvent.AddListener(() =>
        {
            // Shouldn't work if not enough assignments are scanned
            if (GameDirector.Instance.NumAssignmentsScanned <= GameDirector.Instance.NumAssignmentsDone) return;

            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(minigamePrefab, out var minigame);

            // Tell the minigame to run this method when the player finishes
            minigame.OnCompleteMinigame.AddListener(OnCompleteMinigame);
        });
    }

    private void OnCompleteMinigame()
    {
        IsInteractable = true;
        GameDirector.Instance.DoAssignment();
    }
}