using Mirror;
using UnityEngine;

public class Fixable : NetworkBehaviour
{
    public GameObject fixedState;
    public GameObject brokenState;

    [SyncVar] private bool _isBroken;

    [SyncVar] private GameObject _currentState = null;

    private Interactable _interactable;

    public bool IsBroken => _isBroken;

    private void Awake()
    {
        _interactable = gameObject.GetComponent<Interactable>();

        // Since its synced you could join late and its already changed
        if (_currentState != null) _currentState = fixedState;

        //Disable BrokenState, Enable and Cache the FixedState,
        SwitchStateGameObject(fixedState.name);

        //foreach (Transform t in transform)
        //{
        //    if (t.gameObject.name == fixedState.name)
        //    {
        //        t.gameObject.SetActive(true);
        //        _currentState = t.gameObject;
        //    }
        //    else
        //    {
        //        t.gameObject.SetActive(false);
        //    }
        //}

        //if (_interactable != null) ActionManager.FireOnNextUpdate(() => _interactable.IsInteractable = false);
    }

    private void Start()
    {
        UpdateInteractState();
    }

    public void Break()
    => SwitchState(brokenState);

    public void Fix()
    => SwitchState(fixedState);

    private void SwitchState(GameObject state)
    {
        if (!isServer)
        {
            Player.Instance.CmdGiveAuthority(netIdentity);
            ActionManager.RunWhen(() => netIdentity.hasAuthority, () => CmdSwapState(state.name));
        }
        else
        {
            RpcSwapState(state.name);
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

    private void SwitchStateGameObject(string pStateID)
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.name != pStateID)
            {
                t.gameObject.SetActive(false);
            }
            else
            {
                _currentState = t.gameObject;
                t.gameObject.SetActive(true);
            }
        }
        _isBroken = pStateID != fixedState.name;
    }

    private void UpdateInteractState()
    { if (_interactable != null) _interactable.IsInteractable = _isBroken; }
}