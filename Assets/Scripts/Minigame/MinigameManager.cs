using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    private Minigame _currentMinigame;

    private void Awake()
    {
        Instance = this;   
    }

    public void StartMinigame(GameObject minigame, out Minigame returnMinigame)
    {
        var newMinigame = Instantiate(minigame, Camera.main.transform);

        // Weird z stuff to try to get it on top of the background
        var z = Camera.main.nearClipPlane + 0.05f;
        newMinigame.transform.localPosition = new Vector3(0, 0, z);
        newMinigame.transform.localScale = Vector3.one * z / 10f;

        _currentMinigame = newMinigame.GetComponent<Minigame>();
        returnMinigame = _currentMinigame;
    }

    public void StartMinigame(GameObject minigame)
    {
        StartMinigame(minigame, out var _);
    }

    public void StopMinigame()
    {
        // Temporary, should be able to just pause them
        Destroy(_currentMinigame.gameObject);
        _currentMinigame = null;
    }

    public void Update()
    {
        if (InputManager.CurrentInputMode == InputManager.InputMode.Minigame)
        {
            if(InputManager.IsCommandJustPressed(InputManager.InputCommand.Back))
            {
                if (_currentMinigame != null) StopMinigame();
                InputManager.CurrentInputMode = InputManager.InputMode.Player;
            }
        }
    }
}
