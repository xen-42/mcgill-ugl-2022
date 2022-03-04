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

            Ray ray = Camera.main.ScreenPointToRay(InputManager.GetCursorPosition());

            _plane = new Plane(Camera.main.transform.forward, transform.parent.position);
            _plane.Raycast(ray, out float dist);

            Debug.Log(dist);

            var pos = ray.GetPoint(dist);

            //var localPos = transform.parent.transform.InverseTransformPoint(pos);
            //localPos.z = 0;

            transform.position = pos;
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
