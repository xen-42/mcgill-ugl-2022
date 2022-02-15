using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ButtonIconManager : MonoBehaviour
{
    private static ButtonIconManager _instance = null;
    public static ButtonIconManager Instance { get { return _instance; } }

    private Texture2D _missingKey = null;
    private Texture2D _missingMouse = null;
    private readonly Dictionary<Key, Texture2D> _keyboardPrompts = new Dictionary<Key, Texture2D>();
    private readonly Dictionary<GamepadButton, Texture2D> _gamepadPrompts = new Dictionary<GamepadButton, Texture2D>();
    private readonly Dictionary<MouseButton, Texture2D> _mousePrompts = new Dictionary<MouseButton, Texture2D>();

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            // Can only have one
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            Init();
        }
    }

    public static Texture2D GetKeyTexture(Key key)
    {
        if (_instance == null) return null;

        if (_instance._keyboardPrompts.ContainsKey(key)) return _instance._keyboardPrompts[key];

        if (key.ToString().Contains("Mouse")) return _instance._missingMouse;
        else return _instance._missingKey;
    }

    public static Texture2D GetGamepadButtonTexture(GamepadButton gamepadButton)
    {
        if (_instance == null) return null;

        if (_instance._gamepadPrompts.ContainsKey(gamepadButton)) return _instance._gamepadPrompts[gamepadButton];

        return _instance._missingKey;
    }

    public static Texture2D GetMouseButtonTexture(MouseButton button)
    {
        if (_instance == null) return null;

        if (_instance._mousePrompts.ContainsKey(button)) return _instance._mousePrompts[button];

        return _instance._missingMouse;
    }

    private void Init()
    {
        Debug.Log($"Loading {nameof(ButtonIconManager)}");
        _missingKey = Resources.Load<Texture2D>("UI/Prompts/Keyboard & Mouse/Blanks/Blank_Black_Normal");
        _missingMouse = Resources.Load<Texture2D>("UI/Prompts/Keyboard & Mouse/Blanks/Blank_Black_Mouse");

        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            if (_keyboardPrompts.ContainsKey(key))
            {
                Debug.LogWarning($"Keyboard button dictionary alread contains {key}");
                continue;
            }

            var path = "UI/Prompts/Keyboard & Mouse/Dark/" + KeyToFileName(key) + "_Key_Dark";
            var texture = Resources.Load<Texture2D>(path);

            if (texture == null) continue;

            _keyboardPrompts.Add(key, texture);
        } 
        
        foreach(GamepadButton gamepadButton in Enum.GetValues(typeof(GamepadButton))) 
        {
            if (_gamepadPrompts.ContainsKey(gamepadButton)) continue;

            var path = "UI/Prompts/Xbox Series/XBoxSeriesX_" + GamepadButtonToFileName(gamepadButton);
            var texture = Resources.Load<Texture2D>(path);

            if (texture == null) continue;

            _gamepadPrompts.Add(gamepadButton, texture);
        }

        foreach(MouseButton mouseButton in Enum.GetValues(typeof(MouseButton)))
        {
            if (_mousePrompts.ContainsKey(mouseButton)) continue;

            var path = "UI/Prompts/Keyboard & Mouse/Dark/" + MouseButtonToFileName(mouseButton) + "_Key_Dark";
            var texture = Resources.Load<Texture2D>(path);

            if (texture == null) continue;

            _mousePrompts.Add(mouseButton, texture);
        }
    }

    private string KeyToFileName(Key key)
    {
        switch (key)
        {
            case Key.Escape:
                return "Esc";
            case Key.Backquote:
                return "Tilda";
            case Key.Delete:
                return "Del";
            case Key.UpArrow:
                return "Arrow_Up";
            case Key.DownArrow:
                return "Arrow_Down";
            case Key.LeftArrow:
                return "Arrow_Left";
            case Key.RightArrow:
                return "Arrow_Right";
            case Key.PageUp:
                return "Page_Up";
            case Key.PageDown:
                return "Page_Down";
            case Key.NumLock:
                return "Num_Lock";
            case Key.CapsLock:
                return "Caps_Lock";
            case Key.LeftShift:
            case Key.RightShift:
                return "Shift";
            case Key.LeftCtrl:
            case Key.RightCtrl:
                return "Ctrl";
            case Key.LeftAlt:
            case Key.AltGr:
                return "Alt";
            case Key.PrintScreen:
                return "Print_Screen";
            case Key.Backslash:
                return "Back_Slash";
            case Key.NumpadDivide:
                return "Slash";
            case Key.LeftBracket:
                return "Bracket_Left";
            case Key.RightBracket:
                return "Bracket_Right";
            case Key.LeftWindows:
                return "Win";
            case Key.NumpadPlus:
                return "Plus_Tall";
            case Key.NumpadEnter:
                return "Enter_Tall";
            case Key.NumpadMultiply:
                return "Asterisk";
            default:
                var str = key.ToString();
                if (str.StartsWith("Digit")) str = str.Replace("Digit", "");
                else if (str.StartsWith("Numpad")) str = str.Replace("Numpad", "");
                return str;
        }
    }

    private string GamepadButtonToFileName(GamepadButton gamepadButton)
    {
        switch(gamepadButton)
        {
            case GamepadButton.DpadDown:
                return "Dpad_Down";
            case GamepadButton.DpadLeft:
                return "Dpad_Left";
            case GamepadButton.DpadRight:
                return "Dpad_right";
            case GamepadButton.DpadUp:
                return "Dpad_Up";
            case GamepadButton.LeftStick:
                return "Left_Stick_Click";
            case GamepadButton.RightStick:
                return "Right_Stick_Click";
            case GamepadButton.LeftShoulder:
                return "LB";
            case GamepadButton.RightShoulder:
                return "RB";
            case GamepadButton.LeftTrigger:
                return "LT";
            case GamepadButton.RightTrigger:
                return "RT";
            case GamepadButton.Start:
                return "Menu";
            case GamepadButton.Select:
                return "View";
            case GamepadButton.Circle:
                return "B";
            default:
                return gamepadButton.ToString();
        }
    }
    private string MouseButtonToFileName(MouseButton button)
    {
        switch(button)
        {
            case MouseButton.Left:
                return "Mouse_Left";
            case MouseButton.Right:
                return "Mouse_Right";
            case MouseButton.Middle:
                return "Mouse_Middle";
            case MouseButton.Back:
                return "Mouse_4";
            case MouseButton.Forward:
                return "Mouse_5";
            default:
                return button.ToString();
        }
    }
}