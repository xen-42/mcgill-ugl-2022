using System;
using UnityEngine;
using UnityEngine.UI;
using static InputManager;

public class ButtonPrompt : MonoBehaviour
{
    [SerializeField] private Image _radialMeterUI = null;
    [SerializeField] private Text _textUI = null;
    [SerializeField] private Image _image = null;

    public PromptInfo Info { get; private set; }

    private void Awake()
    {
        EventManager.AddListener("ChangedController", OnChangedController);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener("ChangedController", OnChangedController);
    }

    private void OnChangedController()
    {
        // Refresh the sprites
        if (_image?.sprite != null)
        {
            _image.sprite = ButtonIconManager.GetPromptSprite(Info.Command);
        }
    }

    public void OnInit(PromptInfo info)
    {
        Info = info;

        if (Info.HoldTime == 0f)
        {
            // The prompt just needs to be clicked
            _radialMeterUI.gameObject.SetActive(false);
        }

        _image.sprite = ButtonIconManager.GetPromptSprite(Info.Command);
        _textUI.text = Info.Text;
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