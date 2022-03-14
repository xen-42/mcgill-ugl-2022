using Mirror;
using UnityEngine;

public class Fixable : NetworkBehaviour
{
    public GameObject fixedState;
    public GameObject brokenState;

    [SyncVar] private bool _isBroken;

    [SyncVar] private string _currentState = null;

    private Interactable _interactable;

    public bool IsBroken => _isBroken;

    private void Awake()
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

    private void Start()
    {
        UpdateInteractState();
        SwitchState(brokenState.name);
    }

    public void Break()
    => SwitchState(brokenState.name);

    public void Fix()
    {
        SwitchState(fixedState.name);
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

    #region network

    [Command]
    private void CmdSwapState(string stateID)
    {
        RpcSwapState(stateID);
    }

    [ClientRpc]
    private void RpcSwapState(string stateID)
    {
        SwitchStateGameObject(stateID);
        UpdateInteractState();
    }

    #endregion network

    //private void SwitchState(string stateID)
    //{
    //    foreach (Transform t in transform)
    //    {
    //        if (t.gameObject.name != stateID)
    //        {
    //            t.gameObject.SetActive(false);
    //        }
    //        else
    //        {
    //            _currentState = t.gameObject;
    //            t.gameObject.SetActive(true);
    //        }
    //    }
    //    _isBroken = stateID != fixedState.name;

    //    if (_interactable != null) _interactable.IsInteractable = _isBroken;
    //}

    private void SwitchStateGameObject(string stateID)
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
        _isBroken = stateID != fixedState.name;
    }

    private void UpdateInteractState()
    { if (_interactable != null) _interactable.IsInteractable = _isBroken; }
}