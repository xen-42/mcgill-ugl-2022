using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropMinigame : Minigame
{
    [SerializeField]
    public Holdable2D holdableObject;

    [SerializeField]
    public Collider2D dropCollider;
    private Collider2D _heldCollider;

    private void Awake()
    {
        _heldCollider = holdableObject.GetComponent<Collider2D>();    
    }

    void Update()
    {
        // Check for completion
        if(!holdableObject.IsHeld && Physics2D.IsTouching(dropCollider, _heldCollider))
        {
            CompleteMinigame();
        }
    }
}
