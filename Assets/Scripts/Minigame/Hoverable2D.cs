using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable2D : MonoBehaviour
{
    private bool _hovering = false;

    public bool IsCursorHovering()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);

        for (int index = 0; index < raysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = raysastResults[index];
            if (curRaysastResult.gameObject == gameObject)
            {
                if (!_hovering) OnStartHover();
                _hovering = true;
                return true;
            }
        }

        if (_hovering) OnStopHover();
        _hovering = false;
        return false;
    }

    public virtual void OnStartHover() { }

    public virtual void OnStopHover() { }
}
