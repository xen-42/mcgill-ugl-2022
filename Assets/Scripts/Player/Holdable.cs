using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : NetworkBehaviour
{
    [SyncVar]
    private GameObject _parent;

    private Rigidbody _rb;
    private Collider _collider;
    private bool _isSetup = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_parent != null)
        {
            if(!_isSetup)
            {
                _isSetup = true;
                _collider.enabled = false;
                _rb.isKinematic = true;
            }
            transform.position = _parent.transform.position;
            transform.rotation = _parent.transform.rotation;
        }    
        else
        {
            if (_isSetup)
            {
                _isSetup = false;
                _rb.isKinematic = false;
                _collider.enabled = true;
            }
        }
    }

    public void Grab(NetworkBehaviour grabber)
    {
        _parent = grabber.gameObject;
        if(isServer) gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(grabber.connectionToClient);
    }

    public void Drop()
    {
        _parent = null;
        if(isServer) gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
    }
}
