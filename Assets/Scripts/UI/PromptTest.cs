using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class PromptTest : MonoBehaviour
{
    [SerializeField]
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            try
            {
                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    var texture = ButtonIconManager.GetKeyTexture(key);
                    image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2f, texture.height / 2f));
                    Debug.Log($"Key pressed: {key}");
                }
            }
            catch (Exception) { }
        }

        foreach(GamepadButton button in Enum.GetValues(typeof(GamepadButton)))
        {
            try
            {
                if (Gamepad.current[button].wasPressedThisFrame)
                {
                    var texture = ButtonIconManager.GetGamepadButtonTexture(button);
                    image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2f, texture.height / 2f));
                    Debug.Log($"Button pressed: {button}");
                }
            }
            catch (Exception) { }
        }

        foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
        {
            try
            {
                if (Mouse.current[button.ToString().ToLower() + "Button"].IsPressed())
                {
                    var texture = ButtonIconManager.GetMouseButtonTexture(button);
                    image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2f, texture.height / 2f));
                    Debug.Log($"Button pressed: {button}");
                }
            }
            catch (Exception) { }
        }
    }
}
