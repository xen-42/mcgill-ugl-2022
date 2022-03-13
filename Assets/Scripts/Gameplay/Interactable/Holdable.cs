using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;

public class Holdable : Interactable
{
    [SyncVar] private GameObject _parent;
    [SyncVar] private GameObject _offsetPosition;

    private Rigidbody _rb;
    private Collider _collider;

    protected override InputCommand InputCommand { get => InputCommand.PickUp; }

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

        // Input
        if (InputManager.CurrentInputMode != InputManager.InputMode.Player) return;

        var player = Player.Instance;
        if (InputManager.IsCommandJustPressed(InputCommand) && player.heldObject == this)
        {
            player.CmdDrop();
        }
    }

    public void Grab(NetworkBehaviour grabber)
    {
        _parent = grabber.gameObject;

        if (isServer) gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(grabber.connectionToClient);
        IsInteractable = false;
        _collider.enabled = false;
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        _rb.isKinematic = true;
    }

    public void Drop()
    {
        _parent = null;
        if (isServer) gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();

        IsInteractable = true;
        _rb.isKinematic = false;
        _collider.enabled = true;

        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }
    }

    /* Get rid of certain holdable items after using them for a minigame */
    public void Consume()
    {
        // Check if its a consumable type
        var isConsumable = false;
        switch (type)
        {
            case Type.ASSIGNMENT:
                isConsumable = true;
                break;
        }

        if (isConsumable)
        {
            NetworkDestroy(gameObject);
            Player.Instance.heldObject = null;
        }
    }

    private void NetworkDestroy(GameObject obj)
    {
        if (isServer)
        {
            RpcNetworkDestroy(obj);
        }
        else
        {
            CmdNetworkDestroy(obj);
        }
    }

    [Command]
    private void CmdNetworkDestroy(GameObject obj)
    {
        RpcNetworkDestroy(obj);
    }

    [ClientRpc]
    private void RpcNetworkDestroy(GameObject obj)
    {
        GameObject.Destroy(obj);
    }
}
