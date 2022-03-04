using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class Interactable : MonoBehaviour
{
    protected UnityEvent _event = new UnityEvent();

    public void Interact()
    {
        Debug.Log("Interact");
        if(_event != null) _event.Invoke();
    }
}
