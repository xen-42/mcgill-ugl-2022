using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable2D : Interactable
{
    private bool _hovering = false;

    private void OnDestroy()
    {
        if (_hovering) OnStopHover();
    }

    public bool IsCursorHovering()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.GetCursorPosition());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << (LayerMask.NameToLayer("Minigame"))))
        {
            if(hit.collider.transform == transform)
            {
                if (_hovering == false)
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

    public virtual void OnStartHover() { }

    public virtual void OnStopHover() { }
}
