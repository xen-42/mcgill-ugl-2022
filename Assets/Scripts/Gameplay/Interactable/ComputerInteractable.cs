using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ButtonPrompt;
using static InputManager;

public class ComputerInteractable : Interactable
{
    [SerializeField] public GameObject MinigamePrefab;

    [SerializeField] private string _promptTextNothingScanned;
    public PromptInfo NothingScannedPrompt { get; set; }
    private PromptInfo _handsFullPrompt;

    protected override InputCommand InputCommand { get => InputCommand.Interact; }
    [SerializeField] public AudioSource sound;

    void Start()
    {
        // This is janky but I don't want to rework stuff
        NothingScannedPrompt = new PromptInfo(InputCommand.None, _promptTextNothingScanned, 0, 0);
        _handsFullPrompt = WrongItemPrompt;

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

    // Hijacking this to make it work with the computer
    // "Having" the item = enough assignments are scanned + hands are empty
    protected override bool HasItem()
    {
        // Instead of checking for a held item we check the game director for the condition
        if (GameDirector.Instance.NumAssignmentsScanned <= GameDirector.Instance.NumAssignmentsDone)
        {
            WrongItemPrompt = NothingScannedPrompt;
            return false;
        }
        else if (Player.Instance.heldObject != null)
        {
            WrongItemPrompt = _handsFullPrompt;
            return false;
        }

        return true;
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
