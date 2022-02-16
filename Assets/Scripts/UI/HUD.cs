using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private GameObject _gamepadIndicator;

    void Start()
    {
        
    }

    void Update()
    {
        var indicatorShown = _gamepadIndicator.activeInHierarchy;
        var gamepadEnabled = InputManager.IsGamepadEnabled();

        if(indicatorShown == gamepadEnabled)
        {
            _gamepadIndicator.SetActive(!indicatorShown);
        }
    }
}
