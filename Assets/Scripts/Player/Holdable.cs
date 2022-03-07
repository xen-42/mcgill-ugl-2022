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
    private bool _isSetup = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        // When a holdable item is interacting with, pick it up
        _event.AddListener(OnInteract);
    }

    private void OnInteract()
    {
        Debug.Log("Pick up!!!!");
        var player = Player.Instance;
        if (HasFocus && player.heldObject == null)
        {
            player.CmdGrab(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        var player = Player.Instance;

        if(InputManager.IsCommandJustPressed(PromptInfo.Command) && player.heldObject == this)
        {
            player.CmdDrop();
        }

        if (_parent != null)
        {
            transform.position = _parent.transform.position;
            transform.rotation = _parent.transform.rotation;
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
}
