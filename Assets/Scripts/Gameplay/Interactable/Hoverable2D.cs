using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static InputManager;

public class Hoverable2D : Interactable
{
    private bool _hovering = false;

    protected override InputCommand InputCommand { get => InputCommand.None; }

    private void OnDestroy()
    {
        if (_hovering) OnStopHover();
    }

    public bool IsCursorHovering()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.GetCursorPosition());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << (LayerMask.NameToLayer("Minigame"))))
        {
            if (hit.collider.transform == transform)
            {
                if (!_hovering)
                {
                    OnStartHover();
                    GainFocus();
                }

                _hovering = true;
                return true;
            }
        }

        if (_hovering)
        {
            OnStopHover();
            LoseFocus();
        }

        _hovering = false;
        return false;
    }

    public virtual void OnStartHover()
    { }

    public virtual void OnStopHover()
    { }
}