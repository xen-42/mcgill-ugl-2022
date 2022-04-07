using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public static class InputManager
{
    #region Key bindings
    public enum InputCommand
    {
        None,
        Any,
        Interact,
        Jump,
        Sprint,
        PickUp,
        Back,
        Throw
    }

    private static Dictionary<InputCommand, Key> _keyboardMappings = new Dictionary<InputCommand, Key>()
    {
        { InputCommand.Jump, Key.Space },
        { InputCommand.Sprint, Key.LeftShift },
        { InputCommand.Back, Key.Escape }
    };

    private static Dictionary<InputCommand, MouseButton> _mouseMappings = new Dictionary<InputCommand, MouseButton>()
    {
        { InputCommand.Interact, MouseButton.Left },
        { InputCommand.PickUp, MouseButton.Left },
        { InputCommand.Throw, MouseButton.Right }
    };

    public static Dictionary<InputCommand, Key> KeyboardMappings
    {
        get { return _keyboardMappings; }
    }

    public static Dictionary<InputCommand, MouseButton> MouseMapping
    {
        get { return _mouseMappings; }
    }

    #endregion Key bindings

    public enum InputMode
    {
        Player,
        Minigame,
        UI
    }

    // Should be careful when changing this
    private static Image _HUDCursor;
    public static InputMode CurrentInputMode
    {
        get { return _currentInputMode; }
        set
        {
            if (_HUDCursor == null) _HUDCursor = GameObject.Find("HUD/Cursor")?.GetComponent<Image>();

            Debug.Log($"Changing InputMode to {value}");
            _currentInputMode = value;
            if (_currentInputMode == InputMode.Player)
            {
                Cursor.lockState = CursorLockMode.Locked;

                if(_HUDCursor != null) _HUDCursor.enabled = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (_HUDCursor != null) _HUDCursor.enabled = false;
            }
        }
    }
    private static InputMode _currentInputMode = InputMode.UI;

    public static bool IsGamepadEnabled()
    {
        return Gamepad.all.Count > 0;
    }

    public static Vector2 GetMovementAxis()
    {
        Vector2 movement = Vector2.zero;
        if (Keyboard.current != null)
        {
            var left = Keyboard.current.aKey.isPressed ? 1 : 0;
            var right = Keyboard.current.dKey.isPressed ? 1 : 0;
            var up = Keyboard.current.wKey.isPressed ? 1 : 0;
            var down = Keyboard.current.sKey.isPressed ? 1 : 0;

            var keyboardMovement = new Vector2(right - left, up - down);

            movement += keyboardMovement;
        }

        return movement.normalized;
    }

    public static Vector2 GetLookAxis()
    {
        Vector2 movement = Vector2.zero;
        if (Mouse.current != null)
        {
            var keyboardMovement = new Vector2(Mouse.current.delta.x.ReadValue(), Mouse.current.delta.y.ReadValue());

            movement += keyboardMovement;
        }

        return movement;
    }

    public static Vector2 GetCursorPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    #region Pressed
    private static bool IsAnyKeyPressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.IsPressed())
        {
            return true;
        }

        if (Mouse.current != null)
        {
            foreach (var button in Enum.GetValues(typeof(MouseButton)))
            {
                if ((Mouse.current[button.ToString().ToLower() + "Button"] as ButtonControl).wasPressedThisFrame)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsCommandPressed(InputCommand command)
    {
        if (command == InputCommand.None) return false;

        if (command == InputCommand.Any)
        {
            return IsAnyKeyPressed();
        }

        if (_keyboardMappings.ContainsKey(command))
        {
            var key = _keyboardMappings[command];
            if (Keyboard.current != null && Keyboard.current[key].IsPressed()) return true;
        }

        if (_mouseMappings.ContainsKey(command))
        {
            var button = _mouseMappings[command];
            if (Mouse.current != null && Mouse.current[button.ToString().ToLower() + "Button"].IsPressed()) return true;
        }

        return false;
    }

    #endregion Pressed

    #region Just pressed

    private static bool IsAnyKeyJustPressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return true;
        }

        if (Mouse.current != null)
        {
            foreach (InputControl control in Mouse.current.allControls)
            {
                if (!(control is ButtonControl)) continue;
                if ((control as ButtonControl).wasPressedThisFrame)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsCommandJustPressed(InputCommand command)
    {
        if (command == InputCommand.None) return false;

        if (command == InputCommand.Any)
        {
            return IsAnyKeyJustPressed();
        }

        if (_keyboardMappings.ContainsKey(command))
        {
            var key = _keyboardMappings[command];
            if (Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame) return true;
        }

        if (_mouseMappings.ContainsKey(command))
        {
            var button = _mouseMappings[command];
            if (Mouse.current != null && (Mouse.current[button.ToString().ToLower() + "Button"] as ButtonControl).wasPressedThisFrame) return true;
        }

        return false;
    }

    #endregion Just pressed

    #region Just released

    private static bool IsAnyKeyJustReleased()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasReleasedThisFrame)
            return true;

        if (Mouse.current != null)
        {
            foreach (InputControl control in Mouse.current.allControls)
            {
                if (!(control is ButtonControl)) continue;
                if ((control as ButtonControl).wasPressedThisFrame)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsCommandJustReleased(InputCommand command)
    {
        if (command == InputCommand.None) return false;

        if (command == InputCommand.Any)
        {
            return IsAnyKeyJustReleased();
        }

        if (_keyboardMappings.ContainsKey(command))
        {
            var key = _keyboardMappings[command];
            if (Keyboard.current != null && Keyboard.current[key].wasReleasedThisFrame) return true;
        }

        if (_mouseMappings.ContainsKey(command))
        {
            var button = _mouseMappings[command];
            if (Mouse.current != null && (Mouse.current[button.ToString().ToLower() + "Button"] as ButtonControl).wasReleasedThisFrame) return true;
        }

        return false;
    }

    #endregion Just released
}
