using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Holdable2D : Hoverable2D
{
    private RectTransform _rt;
    private bool _isHeld;

    // Start is called before the first frame update
    void Start()
    {
        _rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        var isHovering = IsCursorHovering();

        if(InputManager.IsCommandJustPressed(InputManager.InputCommand.PickUp) && isHovering)
        {
            _isHeld = true;
        }

        if(_isHeld)
        {
            if(InputManager.IsCommandJustReleased(InputManager.InputCommand.PickUp))
            {
                _isHeld = false;
            }

            _rt.position = InputManager.GetCursorPosition();
        }
    }

    public override void OnStartHover()
    {
        EventManager<InputManager.InputCommand>.TriggerEvent("PromptHit", InputManager.InputCommand.PickUp);
    }

    public override void OnStopHover()
    {
        EventManager<InputManager.InputCommand>.TriggerEvent("PromptLost", InputManager.InputCommand.PickUp);
    }
}
