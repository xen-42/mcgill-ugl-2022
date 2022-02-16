using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public void Awake()
    {
        if(_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
    }

    #region Key bindings
    public enum InputCommand
    {
        Any,
        Interact,
        Jump,
        Sprint
    }

    private Dictionary<InputCommand, Key> _keyboardMappings = new Dictionary<InputCommand, Key>() 
    {
        { InputCommand.Interact, Key.E },
        { InputCommand.Jump, Key.Space },
        { InputCommand.Sprint, Key.LeftShift },
    };

    private Dictionary<InputCommand, GamepadButton> _gamepadMappings = new Dictionary<InputCommand, GamepadButton>()
    {
        { InputCommand.Interact, GamepadButton.West },
        { InputCommand.Jump, GamepadButton.South },
        { InputCommand.Sprint, GamepadButton.LeftStick },
    };

    #endregion Key bindings

    // Could extend this later to include other gamepad types
    private enum InputType
    {
        KeyboardAndMouseInput,
        GamepadInput
    }
    private InputType _lastInputType = InputType.KeyboardAndMouseInput;

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

            movement += new Vector2(right - left, up - down);

            _instance._lastInputType = InputType.KeyboardAndMouseInput;
        }
        if (Gamepad.current != null)
        {
            movement += Gamepad.current.leftStick.ReadValue();
            _instance._lastInputType = InputType.GamepadInput;
        }

        return movement.normalized;
    }

    public static Vector2 GetLookAxis()
    {
        Vector2 movement = Vector2.zero;
        if(Mouse.current != null)
        {
            movement += new Vector2(Mouse.current.delta.x.ReadValue(), Mouse.current.delta.y.ReadValue());

            _instance._lastInputType = InputType.KeyboardAndMouseInput;
        }
        if(Gamepad.current != null)
        {
            movement += Gamepad.current.rightStick.ReadValue();
            _instance._lastInputType = InputType.GamepadInput;
        }

        return movement;
    }

    private bool IsGamepadAnyButtonPressed()
    {
        if(Gamepad.current != null)
        {
            foreach(var control in Gamepad.current.allControls)
            {
                if (control.IsPressed()) return true;
            }
        }
        return false;
    }

    private bool IsKeyboardAnyKeyPressed()
    {
        return Keyboard.current != null && Keyboard.current.anyKey.IsPressed();
    }

    private bool IsGamepadButtonPressed(GamepadButton button)
    {
        return Gamepad.current != null && Gamepad.current[button].isPressed;
    }

    private bool IsKeyboardKeyPressed(Key key)
    {
        return Keyboard.current != null && Keyboard.current[key].isPressed;
    }

    public static bool IsCommandPressed(InputCommand command)
    {
        if(command == InputCommand.Any)
            return _instance.IsKeyboardAnyKeyPressed() || _instance.IsGamepadAnyButtonPressed();

        if (_instance._gamepadMappings.ContainsKey(command))
        {
            if (_instance.IsGamepadButtonPressed(_instance._gamepadMappings[command])) return true;
        }
        
        if (_instance._keyboardMappings.ContainsKey(command))
        {
            if (_instance.IsKeyboardKeyPressed(_instance._keyboardMappings[command])) return true;
        }

        return false;
    }
}
