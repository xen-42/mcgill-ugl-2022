using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : Interactable
{
    [SyncVar] private GameObject _parent;
    [SyncVar] private GameObject _offsetPosition;

    #region Caches

    private Rigidbody _rb;
    private Collider _collider;

    #endregion Caches

    public Type type;

    /// <summary>
    /// Type of Holdable
    /// </summary>
    public enum Type
    {
        NONE,
        WATERING_CAN,
        ASSIGNMENT
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        // When a holdable item is interacting with, pick it up
        _unityEvent.AddListener(OnInteract);
    }

    private void OnInteract()
    {
        var player = Player.Instance;
        if (HasFocus && player.heldObject == null)
        {
            player.CmdGrab(this);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Input
        if (InputManager.CurrentInputMode != InputManager.InputMode.Player) return;

        var player = Player.Instance;
        if (InputManager.IsCommandJustPressed(PromptInfo.Command) && player.heldObject == this)
        {
            player.CmdDrop();
        }
    }

    #region Actions Called by Player

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

    #endregion Actions Called by Player

    /// <summary>
    /// Get rid of certain holdable items after using them for a minigame
    /// </summary>
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

    #region Network

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
        Destroy(obj);
    }

    #endregion Network
}