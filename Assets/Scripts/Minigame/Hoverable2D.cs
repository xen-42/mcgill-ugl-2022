using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable2D : MonoBehaviour
{
    private bool _hovering = false;

    public bool IsCursorHovering()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.GetCursorPosition());
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider?.transform == transform)
        {
            if(_hovering == false) OnStartHover();
            _hovering = true;
            return true;
        }

        if (_hovering) OnStopHover();
        _hovering = false;
        return false;
    }

    public virtual void OnStartHover() { }

    public virtual void OnStopHover() { }
}
