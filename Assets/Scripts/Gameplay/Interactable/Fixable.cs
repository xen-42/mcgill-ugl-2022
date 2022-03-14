using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixable : NetworkBehaviour
{
    public event Action OnObjectFix = delegate { };

    public event Action OnObjectBreak = delegate { };

    [SerializeField] public GameObject fixedState;

    [SerializeField] public GameObject brokenState;

    [SyncVar] private bool _isBroken;

    public bool IsBroken { get => _isBroken; }

    [SyncVar] private string _currentState = null;

    private Interactable _interactable;

    private void Start()
    {
        // Since its synced you could join late and its already changed
        if (_currentState == null) _currentState = fixedState.name;

        foreach (Transform t in transform)
        {
            if (t.gameObject.name == _currentState)
            {
                t.gameObject.SetActive(true);
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }

        _isBroken = _currentState != fixedState.name;

        _interactable = gameObject.GetComponent<Interactable>();
        _interactable.Init(_isBroken);
    }

    public void Break()
    {
        SwitchState(brokenState.name);
        OnObjectBreak.Invoke();
    }

    public void Fix()
    {
        SwitchState(fixedState.name);
        OnObjectFix.Invoke();
    }

    private void SwitchState(string stateID)
    {
        if (!isServer)
        {
            Player.Instance.CmdGiveAuthority(netIdentity);
            ActionManager.RunWhen(() => netIdentity.hasAuthority, () => CmdSwapState(stateID));
        }
        else
        {
            RpcSwapState(stateID);
        }
    }

    [Command]
    private void CmdSwapState(string stateID)
    {
        RpcSwapState(stateID);
    }

    [ClientRpc]
    private void RpcSwapState(string stateID)
    {
        _SwitchState(stateID);
    }

    private void _SwitchState(string stateID)
    {
        _currentState = stateID;

        foreach (Transform t in transform)
        {
            if (t.gameObject.name != stateID)
            {
                t.gameObject.SetActive(false);
            }
            else
            {
                t.gameObject.SetActive(true);
            }
        }
        _isBroken = (stateID != fixedState.name);

        if (_interactable != null) _interactable.IsInteractable = _isBroken;
    }
}