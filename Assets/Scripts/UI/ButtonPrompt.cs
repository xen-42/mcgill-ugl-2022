using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InputManager;
using static ButtonIconManager;
using UnityEngine.Events;

public class ButtonPrompt : MonoBehaviour
{
    [SerializeField] private Image _radialMeterUI = null;
    [SerializeField] private Text _textUI = null;
    [SerializeField] private Image _image = null;

    public PromptInfo Info { get; private set; }

    private void Awake()
    {
        EventManager.AddListener("ChangedController", RefreshSprite);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener("ChangedController", RefreshSprite);
    }

    public void Init(PromptInfo info)
    {
        Info = info;

        if (Info.HoldTime == 0f)
        {
            // The prompt just needs to be clicked
            _radialMeterUI.gameObject.SetActive(false);
        }

        Debug.Log(info.Text);

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
    }

    public struct PromptInfo
    {
        public InputCommand Command { get; set; }
        public string Text { get; set; }
        public int Priority { get; private set; }
        public float HoldTime { get; private set; }

        public PromptInfo(InputCommand cmd, string text, int priority, float holdTime)
        {
            Command = cmd;
            Text = text;
            Priority = priority;
            HoldTime = holdTime;
        }
    }
}