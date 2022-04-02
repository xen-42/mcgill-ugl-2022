using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InputManager;
using static ButtonIconManager;
using UnityEngine.Events;
using System;

public class ButtonPrompt : MonoBehaviour
{
    [SerializeField] private Text _textUI = null;
    [SerializeField] private Image _image = null;

    public PromptInfo Info { get => _info; }

    [SerializeField] private PromptInfo _info;

    private void Awake()
    {
        EventManager.AddListener("ChangedController", RefreshSprite);

        RefreshSprite();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener("ChangedController", RefreshSprite);
    }

    public void Init(PromptInfo info)
    {
        _info = info;

        _textUI.text = Info.Text;

        RefreshSprite();
    }

    private void RefreshSprite()
    {
        if (_image != null)
        {
            _image.sprite = GetPromptSprite(Info.Command);
            _image.gameObject.SetActive(_image.sprite != null);
        }
        _textUI.text = _info.Text;
    }

    private void OnValidate()
    {
        RefreshSprite();
    }

    [Serializable]
    public struct PromptInfo
    {
        public InputCommand Command;
        public string Text;

        public PromptInfo(InputCommand cmd, string text)
        {
            Command = cmd;
            Text = text;
        }
    }
}