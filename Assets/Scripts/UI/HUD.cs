using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private GameObject _gamepadIndicator;

    [SerializeField]
    private GameObject _buttonPrompt;

    private Dictionary<InputManager.InputCommand, GameObject> _buttonPromptDict;

    void Start()
    {
        EventManager<InputManager.InputCommand>.AddListener("PromptHit", OnPromptHit);
        EventManager<InputManager.InputCommand>.AddListener("PromptLost", OnPromptLost);

        _buttonPromptDict = new Dictionary<InputManager.InputCommand, GameObject>();
    }

    private void OnDestroy()
    {
        EventManager<InputManager.InputCommand>.RemoveListener("PromptHit", OnPromptHit);
        EventManager<InputManager.InputCommand>.RemoveListener("PromptLost", OnPromptLost);
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

    private void OnPromptHit(InputManager.InputCommand command)
    {
        // TODO replace this with a buttonprompt object that automatically changes based on the controller type
        if(_buttonPromptDict.TryGetValue(command, out GameObject obj))
        {
            obj.SetActive(true);
        }
        else
        {
            var newPrompt = GameObject.Instantiate(_buttonPrompt, _buttonPrompt.transform.parent);
            newPrompt.GetComponent<Image>().sprite = ButtonIconManager.GetPromptSprite(command);

            var currentPrompts = _buttonPromptDict.Values.ToArray();
            if(currentPrompts.Length > 0)
            {
                RectTransform rect = newPrompt.GetComponent<RectTransform>();

                var pos = rect.localPosition;
                pos.y = currentPrompts.Length * -rect.sizeDelta.y;
                rect.localPosition = pos;
            }

            _buttonPromptDict.Add(command, newPrompt);
            newPrompt.SetActive(true);
        }        
    }

    private void OnPromptLost(InputManager.InputCommand command)
    {
        if (_buttonPromptDict.TryGetValue(command, out GameObject obj))
        {
            _buttonPromptDict.Remove(command);
            GameObject.Destroy(obj);

            // Reorder the remaining ones
            int i = 0;
            foreach(var prompt in _buttonPromptDict.Values.ToArray())
            {
                RectTransform rect = prompt.GetComponent<RectTransform>();
                var pos = rect.localPosition;
                pos.y = i++ * -rect.sizeDelta.y;
                rect.localPosition = pos;
            }
        }
    }
}
