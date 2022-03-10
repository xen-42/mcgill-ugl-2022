using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixable : NetworkBehaviour
{
    [SerializeField]
    public GameObject fixedState;

    [SerializeField]
    public GameObject brokenState;

    [SyncVar] private bool _isBroken;

    [SyncVar] private GameObject _currentState = null;

    private Interactable _interactable;

    private void Start()
    {
        // Since its synced you could join late and its already changed
        if(_currentState != null) _currentState = fixedState;

        foreach(Transform t in transform)
        {
            if (t.gameObject.name == fixedState.name)
            {
                t.gameObject.SetActive(true);
                _currentState = t.gameObject;
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }

        _interactable = gameObject.GetComponent<Interactable>();
        //if (_interactable != null) ActionManager.FireOnNextUpdate(() => _interactable.IsInteractable = false);
    }

    public void Break()
    {
        SwitchState(brokenState);
    }

    public void Fix()
    {
        SwitchState(fixedState);
    }

    private void SwitchState(GameObject state)
    {
        if(!isServer)
        {
            Player.Instance.CmdGiveAuthority(netIdentity);
            ActionManager.RunWhen(() => netIdentity.hasAuthority, () => CmdSwapState(state.name));
        }
        else
        {
            RpcSwapState(state.name);
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
        foreach(Transform t in transform)
        {
            if (t.gameObject.name != stateID)
            {
                t.gameObject.SetActive(false);
            }
            else
            {
                _currentState = t.gameObject;
                t.gameObject.SetActive(true);
            }
        }
        _isBroken = (stateID != fixedState.name);

        if (_interactable != null) _interactable.IsInteractable = _isBroken;
    }

    public bool IsBroken()
    {
        return _isBroken;
    }
}
