using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerMinigame : DragDropMinigame
{
    [SerializeField] public GameObject openScanner;
    [SerializeField] public GameObject closedScanner;

    [SerializeField] public float delayTime = 1f;
    private float _delay = 0f;
    private bool _completed;

    public void Update()
    {
        if (_completed)
        {
            _delay += Time.deltaTime;
            if(_delay > delayTime)
            {
                CompleteMinigame();
            }
        }
        else
        {
            // Check for completion
            if (!holdableObject.IsHeld && dropCollider.bounds.Intersects(_heldCollider.bounds))
            {
                ScanItem();
            }
        }
    }

    public void ScanItem()
    {
        // Get rid of the paper and close the scanner
        holdableObject.gameObject.SetActive(false);

        openScanner.SetActive(false);
        closedScanner.SetActive(true);

        _completed = true;

        // The assignment was stolen if its colour doesnt match the player
        var assignmentColour = Player.Instance.heldObject?.colour;
        var playerColour = Player.Instance.colour;

        if(assignmentColour != null && assignmentColour != playerColour)
        {
            StatTracker.Instance.OnStealAssignment();
        }

        EventManager.TriggerEvent("MinigameComplete");
        OnCompleteMinigame.Invoke();
    }

    public override void CompleteMinigame()
    {
        InputManager.CurrentInputMode = InputManager.InputMode.Player;

        // Once a minigame is completed we just dispose of it
        if (gameObject != null) Destroy(gameObject);
    }
}
