using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : NetworkBehaviour
{
    [SyncVar]
    public GameObject Parent;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Parent != null)
        {
            _rb.isKinematic = true;
            transform.position = Parent.transform.position;
            transform.rotation = Parent.transform.rotation;
        }    
    }
}
