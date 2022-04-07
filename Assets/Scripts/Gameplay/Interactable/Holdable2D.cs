using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputManager;

public class Holdable2D : Hoverable2D
{
    private bool _isHeld;
    public bool IsHeld { get { return _isHeld; } }
    private Plane _plane;

    public AudioSource clickSound;

    protected override InputCommand InputCommand { get => InputCommand.PickUp; }

    void Start()
    {
        clickSound = GameObject.Find("AudioManager").GetComponent<AudioSource>();
    }

    void Update()
    {
        var isHovering = IsCursorHovering();

        if (InputManager.IsCommandJustPressed(InputCommand.PickUp) && isHovering)
        {
            _isHeld = true;
            clickSound.Play();
        }

        if (_isHeld)
        {
            if (InputManager.IsCommandJustReleased(InputCommand.PickUp))
            {
                _isHeld = false;
            }

            Ray ray = Camera.main.ScreenPointToRay(GetCursorPosition());

            _plane = new Plane(Camera.main.transform.forward, transform.parent.position);
            _plane.Raycast(ray, out float dist);

            var pos = ray.GetPoint(dist);

            transform.position = pos;
        }
    }

    public override void OnStartHover()
    {
        EventManager<ButtonPrompt.PromptInfo>.TriggerEvent("PromptHit", InteractablePrompt);
    }

    public override void OnStopHover()
    {
        EventManager<ButtonPrompt.PromptInfo>.TriggerEvent("PromptLost", InteractablePrompt);
    }
}
