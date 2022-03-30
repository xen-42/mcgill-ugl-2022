using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    private Minigame _currentMinigame;
    private Interactable _currentInteractable;
    private Vector3 start_pos;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMinigame(Interactable interactable, GameObject minigame, out Minigame returnMinigame)
    {
        var newMinigame = Instantiate(minigame, Camera.main.transform);

        // Weird z stuff to try to get it on top of the background
        var z = Camera.main.nearClipPlane + 0.05f;
        newMinigame.transform.localPosition = new Vector3(0, 0, z);
        // Dividing by 10 because when testing minigames the camera defaults to being at that distance
        newMinigame.transform.localScale = Vector3.one * z / 10f;

        start_pos = Player.Instance.transform.position;
        _currentMinigame = newMinigame.GetComponent<Minigame>();
        _currentInteractable = interactable;
        returnMinigame = _currentMinigame;
    }

    public void StartMinigame(Interactable interactable, GameObject minigame)
    {
        // If we don't care about returning the reference to the minigame
        StartMinigame(interactable, minigame, out var _);
    }

    public void StopMinigame()
    {
        // Destroy the minigame
        Destroy(_currentMinigame.gameObject);
        _currentMinigame = null;

        // Reset the object interacted with if need be
        if(_currentInteractable.resetAfterUse) _currentInteractable.IsInteractable = true;
        _currentInteractable = null;

        InputManager.CurrentInputMode = InputManager.InputMode.Player;
    }

    public void Update()
    {
        // If the player moves away from the minigame, the minigame stops and the player has to start from the beginning
        if(_currentMinigame != null)
        {
            var dx = Mathf.Abs(Player.Instance.transform.position.x - start_pos.x);
            var dz = Mathf.Abs(Player.Instance.transform.position.z - start_pos.z);

            // Also check if the player pressed the button to quit out of the minigame
            if (dx >= 1 || dz >= 1 || InputManager.IsCommandJustPressed(InputManager.InputCommand.Back))
            {
                StopMinigame();
            }
        }
    }
}
