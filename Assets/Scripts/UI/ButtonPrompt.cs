using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void Init(PromptInfo info)
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

    public struct PromptInfo
    {
        public InputManager.InputCommand Command { get; private set; }
        public string Text { get; private set; }
        public int Priority { get; private set; }
        public float HoldTime { get; private set; }

        public PromptInfo(InputManager.InputCommand cmd, string text, int priority, float holdTime)
        {
            Command = cmd;
            Text = text;
            Priority = priority;
            HoldTime = holdTime;
        }
    }
}
