using System;
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

    public void OnInit(PromptInfo Info)
    {
        this.Info = Info;

        if (Info.HoldTime == 0f)
        {
            // The prompt just needs to be clicked
            _radialMeterUI.gameObject.SetActive(false);
        }
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

    /// <summary>
    /// Which command, what text, priority, hold Time
    /// </summary>
    [Serializable]
    public struct PromptInfo
    {
        [SerializeField] private InputCommand _command;
        [SerializeField] private string _text;
        [SerializeField] private int _priority;
        [SerializeField] private float _holdTime;
        public InputCommand Command => _command;
        public string Text => _text;
        public int Priority => _priority;
        public float HoldTime => _holdTime;

        public PromptInfo(InputCommand cmd, string text, int priority, float holdTime)
        {
            _command = cmd;
            _text = text;
            _priority = priority;
            _holdTime = holdTime;
        }
    }
}