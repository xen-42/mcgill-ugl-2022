using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteractable : Interactable
{
    [SerializeField]
    public UnityEvent OnInteract = new UnityEvent();

    void Start()
    {
        _unityEvent.AddListener(OnInteract.Invoke);
        if (requiredObject != Holdable.Type.NONE)
        {
            // Try consuming the required object after use
            _unityEvent.AddListener(() => Player.Instance.heldObject.Consume());
        }
    }
}
