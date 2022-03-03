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

    private void Awake()
    {
        _plane = new Plane(transform.forward, transform.position);      
    }

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

            var mousePos = InputManager.GetCursorPosition();
            var zDistance = Math.Abs(_plane.GetDistanceToPoint(Camera.main.transform.position));
            var screenPos = new Vector3(mousePos.x, mousePos.y, zDistance);

            transform.localPosition = Camera.main.ScreenToWorldPoint(screenPos);
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
