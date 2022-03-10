using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : Interactable
{
    [SyncVar]
    private GameObject _parent;

    private Rigidbody _rb;
    private Collider _collider;

    public Type type;

    public enum Type
    {
        NONE,
        WATERING_CAN,
        ASSIGNMENT
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        // When a holdable item is interacting with, pick it up
        _unityEvent.AddListener(OnInteract);
    }

    public void OnInteract()
    {
        var player = Player.Instance;
        if (HasFocus && player.heldObject == null)
        {
            player.CmdGrab(this);
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (_parent != null)
        {
            transform.position = _parent.transform.position;
            transform.rotation = _parent.transform.rotation;
        }

        // Input
        if (InputManager.CurrentInputMode != InputManager.InputMode.Player) return;

        var player = Player.Instance;
        if (InputManager.IsCommandJustPressed(PromptInfo.Command) && player.heldObject == this)
        {
            player.CmdDrop();
        }
    }

    public void Grab(NetworkBehaviour grabber)
    {
        _parent = grabber.gameObject;
        if(isServer) gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(grabber.connectionToClient);
        IsInteractable = false;
        _collider.enabled = false;
        _rb.isKinematic = true;
    }

    public void Drop()
    {
        _parent = null;
        if(isServer) gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        IsInteractable = true;
        _rb.isKinematic = false;
        _collider.enabled = true;
    }

    /* Get rid of certain holdable items after using them for a minigame */
    public void Consume()
    {
        // Check if its a consumable type
        var isConsumable = false;
        switch(type)
        {
            case Type.ASSIGNMENT:
                isConsumable = true;
                break;
        }

        if(isConsumable)
        {
            Player.Instance.CmdDrop();
            ActionManager.RunWhen(() => Player.Instance.heldObject == null, () => Destroy(gameObject));
        }
    }
}
