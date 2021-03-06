using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InputManager;

public class SimpleInteractable : Interactable
{
    [SerializeField]
    public UnityEvent OnInteract = new UnityEvent();

    protected override InputCommand InputCommand { get => InputCommand.Interact; }

    void Start()
    {
        _unityEvent.AddListener(() =>
        {
            OnInteract.Invoke();
            if (interactionEvent != null)
            {
                EventManager.TriggerEvent(interactionEvent);
            }
            if (requiredObject != Holdable.Type.NONE)
            {
                // Try consuming the required object after use
                Player.Instance.heldObject.Consume();
            }
        });
    }
}
