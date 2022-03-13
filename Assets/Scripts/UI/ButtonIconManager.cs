using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class ButtonIconManager
{
    #region Cache Textures for Device Input

    private static readonly Dictionary<Key, Texture2D> _keyboardPrompts = new Dictionary<Key, Texture2D>();
    private static readonly Dictionary<GamepadButton, Texture2D> _gamepadPrompts = new Dictionary<GamepadButton, Texture2D>();
    private static readonly Dictionary<MouseButton, Texture2D> _mousePrompts = new Dictionary<MouseButton, Texture2D>();

    #endregion Cache Textures for Device Input

    #region Cache Textures when a Key-Texture mapping is missing

    private static Texture2D _missingKey = null;
    private static Texture2D _missingMouse = null;

    #endregion Cache Textures when a Key-Texture mapping is missing

    #region Initialization

    /// <summary>
    /// Does nothing but trying to call it will trigger the constructor
    /// </summary>
    public static void Init()
    { }

    /// <summary>
    /// Static Constructor for the class
    ///<para>Simply Loading some Assets</para>
    /// </summary>
    static ButtonIconManager()
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

        foreach (GamepadButton gamepadButton in Enum.GetValues(typeof(GamepadButton)))
        {
            if (_gamepadPrompts.ContainsKey(gamepadButton)) continue;

            var path = "UI/Prompts/Xbox Series/XBoxSeriesX_" + GamepadButtonToFileName(gamepadButton);
            var texture = Resources.Load<Texture2D>(path);

            if (texture == null) continue;

            _gamepadPrompts.Add(gamepadButton, texture);
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

    #endregion Initialization

    #region Convert Device Input to Texture

    public static Texture2D GetKeyTexture(Key key)
        => _keyboardPrompts.ContainsKey(key) ? _keyboardPrompts[key] : key.ToString().Contains("Mouse") ? _missingMouse : _missingKey;

    public static Texture2D GetGamepadButtonTexture(GamepadButton gamepadButton)
       => _gamepadPrompts.ContainsKey(gamepadButton) ? _gamepadPrompts[gamepadButton] : _missingKey;

    public static Texture2D GetMouseButtonTexture(MouseButton button)
    => _mousePrompts.ContainsKey(button) ? _mousePrompts[button] : _missingMouse;

    #endregion Convert Device Input to Texture

    /// <summary>
    /// Get the Prompt Sprite of the Command
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static Sprite GetPromptSprite(InputManager.InputCommand command)
    {
        Texture2D texture = null;

        //Check which Device Input the command is binding with and Get the According Texture
        if (InputManager.IsUsingGamepad() && InputManager.GamepadMapping.ContainsKey(command))
        {
            texture = GetGamepadButtonTexture(InputManager.GamepadMapping[command]);
        }
        else if (InputManager.KeyboardMappings.ContainsKey(command))
        {
            texture = GetKeyTexture(InputManager.KeyboardMappings[command]);
        }
        else if (InputManager.MouseMapping.ContainsKey(command))
        {
            texture = GetMouseButtonTexture(InputManager.MouseMapping[command]);
        }
        else
        {
            //Couldn't Find a binding for the command
            Debug.LogError($"Couldn't get sprite for {command}");
            return null;
        }

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2f, texture.height / 2f));
    }

    #region Convert Device Input to File Name

    /// <summary>
    /// Get the File Name of the key input
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get the File Name of the GamePad Button
    /// </summary>
    /// <param name="gamepadButton"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get the File Name of the Mouse Button Input
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
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

    #endregion Convert Device Input to File Name
}