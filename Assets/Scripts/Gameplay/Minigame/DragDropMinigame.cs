using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropMinigame : Minigame
{
    [SerializeField] public Holdable2D holdableObject;

    [SerializeField] public Collider dropCollider;
    protected Collider _heldCollider;

    private void Awake()
    {
        _heldCollider = holdableObject.GetComponent<Collider>();
    }

    void Update()
    {
        // Check for completion
        if (!holdableObject.IsHeld && dropCollider.bounds.Intersects(_heldCollider.bounds))
        {
            CompleteMinigame();
        }
    }
}
