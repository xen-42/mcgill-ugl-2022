using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Holdable2D : Hoverable2D
{
    private bool _isHeld;
    public bool IsHeld { get { return _isHeld; } }
    private Plane _plane;

    void Update()
    {
        var isHovering = IsCursorHovering();

        if (InputManager.IsCommandJustPressed(InputManager.InputCommand.PickUp) && isHovering)
        {
            _isHeld = true;
        }

        if (_isHeld)
        {
            if (InputManager.IsCommandJustReleased(InputManager.InputCommand.PickUp))
            {
                _isHeld = false;
            }

            Ray ray = Camera.main.ScreenPointToRay(InputManager.GetCursorPosition());

            _plane = new Plane(Camera.main.transform.forward, transform.parent.position);
            _plane.Raycast(ray, out float dist);

            var pos = ray.GetPoint(dist);

            transform.position = pos;
        }
    }

    public override void OnStartHover()
    {
        EventManager<ButtonPrompt.PromptInfo>.TriggerEvent("PromptHit", PromptInfo);
    }

    public override void OnStopHover()
    {
        EventManager<ButtonPrompt.PromptInfo>.TriggerEvent("PromptLost", PromptInfo);
    }
}
