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
    [SerializeField] public AudioSource sound;

    void Start()
    {
        // When the player interacts with this object it'll start the minigame
        _unityEvent.AddListener(() =>
        {
            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(this, MinigamePrefab, out var minigame);

            // Tell the minigame to run this method when the player finishes
            minigame.OnCompleteMinigame.AddListener(OnCompleteMinigame);
        });

        resetAfterUse = true;
    }

    protected override bool HasItem()
    {
        // Instead of checking for a held item we check the game director for the condition
        return (GameDirector.Instance.NumAssignmentsScanned > GameDirector.Instance.NumAssignmentsDone);
    }

    private void OnCompleteMinigame()
    {
        if (sound != null){
            sound.Play();
        }
        IsInteractable = true;
        GameDirector.Instance.DoAssignment();
    }
}
