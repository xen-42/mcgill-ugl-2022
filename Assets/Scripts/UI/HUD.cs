using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private GameObject _gamepadIndicator;

    [SerializeField]
    private GameObject _buttonPrompt;

    void Start()
    {
        EventManager.Instance.AddListener("InteractableObjectHit", OnInteractibleObjectHit);    
        EventManager.Instance.AddListener("InteractableObjectLost", OnInteractibleObjectLost);    
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener("InteractableObjectHit", OnInteractibleObjectHit);
        EventManager.Instance.RemoveListener("InteractableObjectLost", OnInteractibleObjectLost);
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

    private void OnInteractibleObjectHit()
    {
        // TODO replace this with a buttonprompt object that automatically changes based on the controller type
        _buttonPrompt.GetComponent<Image>().sprite = ButtonIconManager.GetPromptSprite(InputManager.InputCommand.Interact);
        _buttonPrompt.SetActive(true);
    }

    private void OnInteractibleObjectLost()
    {
        _buttonPrompt.SetActive(false);
    }
}
