using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadCursor : MonoBehaviour
{
    public static GamepadCursor Instance
    {
        get { return _instance; }
    }
    private static GamepadCursor _instance;

    private Vector2 _position;
    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; Mouse.current.WarpCursorPosition(_position); }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("There is more than one GamepadCursor");
            GameObject.Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    void Update()
    {
        if (InputManager.IsUsingGamepad())
        {
            _position += InputManager.GetMovementAxis();
            Mouse.current.WarpCursorPosition(_position);
        }
        else
        {
            _position = Mouse.current.position.ReadValue();
        }
    }
}
