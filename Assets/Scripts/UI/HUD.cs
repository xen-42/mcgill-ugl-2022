using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject _gamepadIndicator;

    [SerializeField] private GameObject _buttonPrompt;

    private Dictionary<ButtonPrompt.PromptInfo, ButtonPrompt> _buttonPromptDict;

    void Start()
    {
        EventManager<ButtonPrompt.PromptInfo>.AddListener("PromptHit", OnPromptHit);
        EventManager<ButtonPrompt.PromptInfo>.AddListener("PromptLost", OnPromptLost);

        ButtonIconManager.Init();

        _buttonPromptDict = new Dictionary<ButtonPrompt.PromptInfo, ButtonPrompt>();
    }

    private void OnDestroy()
    {
        EventManager<ButtonPrompt.PromptInfo>.RemoveListener("PromptHit", OnPromptHit);
        EventManager<ButtonPrompt.PromptInfo>.RemoveListener("PromptLost", OnPromptLost);
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

    private void OnPromptHit(ButtonPrompt.PromptInfo promptInfo)
    {
        if(_buttonPromptDict.TryGetValue(promptInfo, out ButtonPrompt buttonPrompt))
        {
            buttonPrompt.gameObject.SetActive(true);
        }
        else
        {
            var newPrompt = GameObject.Instantiate(_buttonPrompt, _buttonPrompt.transform.parent);
            newPrompt.GetComponent<ButtonPrompt>().Init(promptInfo);

            _buttonPromptDict.Add(promptInfo, newPrompt.GetComponent<ButtonPrompt>());
            newPrompt.SetActive(true);

            SortPrompts();
        }        
    }

    private void OnPromptLost(ButtonPrompt.PromptInfo promptInfo)
    {
        if (_buttonPromptDict.TryGetValue(promptInfo, out ButtonPrompt buttonPrompt))
        {
            _buttonPromptDict.Remove(promptInfo);
            GameObject.Destroy(buttonPrompt.gameObject);

            SortPrompts();
        }
    }

    private void SortPrompts()
    {
        // Reorder the remaining ones
        int i = 0;
        var sortedPrompts = _buttonPromptDict.Values.OrderBy(x => x.Info.Priority);
        foreach (var prompt in _buttonPromptDict.Values.ToArray())
        {
            RectTransform rect = prompt.GetComponent<RectTransform>();
            var pos = rect.localPosition;
            pos.y = i++ * -rect.sizeDelta.y;
            rect.localPosition = pos;
        }
    }
}
