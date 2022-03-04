using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteractable : Interactable
{
    [SerializeField]
    public UnityEvent OnInteract;

    void Start()
    {
        _event.AddListener(OnInteract.Invoke);
    }
}
