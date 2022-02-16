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
        if(!_gamepadIndicator.activeInHierarchy && InputManager.IsGamepadEnabled())
        {
            _gamepadIndicator.SetActive(false);
        }
        if (_gamepadIndicator.activeInHierarchy && !InputManager.IsGamepadEnabled())
        {
            _gamepadIndicator.SetActive(true);
        }
    }
}
