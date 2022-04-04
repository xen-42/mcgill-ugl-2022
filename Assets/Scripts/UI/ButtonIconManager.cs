using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class ButtonIconManager
{
    private static Texture2D _missingKey = null;
    private static Texture2D _missingMouse = null;
    private static readonly Dictionary<Key, Texture2D> _keyboardPrompts = new Dictionary<Key, Texture2D>();
    private static readonly Dictionary<MouseButton, Texture2D> _mousePrompts = new Dictionary<MouseButton, Texture2D>();

    // Does nothing but trying to call it will trigger the constructor
    public static void Init() { }

    public static Texture2D GetKeyTexture(Key key)
    {
        if (_keyboardPrompts.ContainsKey(key)) return _keyboardPrompts[key];

        if (key.ToString().Contains("Mouse")) return _missingMouse;
        else return _missingKey;
    }

    public static Texture2D GetMouseButtonTexture(MouseButton button)
    {
        if (_mousePrompts.ContainsKey(button)) return _mousePrompts[button];

        return _missingMouse;
    }

    public static Sprite GetPromptSprite(InputManager.InputCommand command)
    {
        if (command == InputManager.InputCommand.None) return null;

        Texture2D texture = null;
        if (InputManager.KeyboardMappings.ContainsKey(command))
        {
            texture = GetKeyTexture(InputManager.KeyboardMappings[command]);
        }
        else if (InputManager.MouseMapping.ContainsKey(command))
        {
            texture = GetMouseButtonTexture(InputManager.MouseMapping[command]);
        }

        if (texture == null)
        {
            Debug.LogWarning($"Couldn't get sprite for {command}");
            return null;
        }

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2f, texture.height / 2f));
    }

    static ButtonIconManager()
    {
        Debug.Log($"Loading {nameof(ButtonIconManager)}");
        _missingKey = Resources.Load<Texture2D>("UI/Prompts/Keyboard & Mouse/Blanks/Blank_Black_Normal");
        _missingMouse = Resources.Load<Texture2D>("UI/Prompts/Keyboard & Mouse/Blanks/Blank_Black_Mouse");

        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            if (_keyboardPrompts.ContainsKey(key))
            {
                //Debug.LogWarning($"Keyboard button dictionary alread contains {key}");
                continue;
            }

            var path = "UI/Prompts/Keyboard & Mouse/Dark/" + KeyToFileName(key) + "_Key_Dark";
            var texture = Resources.Load<Texture2D>(path);

            if (texture == null) continue;

            _keyboardPrompts.Add(key, texture);
        }

        foreach (MouseButton mouseButton in Enum.GetValues(typeof(MouseButton)))
        {
            if (_mousePrompts.ContainsKey(mouseButton)) continue;

            var path = "UI/Prompts/Keyboard & Mouse/Dark/" + MouseButtonToFileName(mouseButton) + "_Key_Dark";
            var texture = Resources.Load<Texture2D>(path);

            if (texture == null) continue;

            _mousePrompts.Add(mouseButton, texture);
        }
    }

    private static string KeyToFileName(Key key)
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

    private static string GamepadButtonToFileName(GamepadButton gamepadButton)
    {
        switch (gamepadButton)
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
    private static string MouseButtonToFileName(MouseButton button)
    {
        switch (button)
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
