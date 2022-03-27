using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixable : NetworkBehaviour
{
    [SerializeField] public GameObject fixedState;
    [SerializeField] public GameObject brokenState;

    private string _fixedName;
    private string _brokenName;

    [SyncVar] private bool _isBroken;

    public bool CanBreak { get => _isBroken && _cooldown == 0f; }

    [SyncVar] private string _currentState = null;

    private Interactable _interactable;

    [SerializeField] public string brokenStateSound;
    [SerializeField] public string fixedStateSound;

    // Cooldown to prevent breaking right after fixing it
    [SerializeField] public float brokenCooldown = 5f;
    private float _cooldown;

    private void Awake()
    {
        _fixedName = fixedState.name;
        _brokenName = brokenState.name;

        // Since its synced you could join late and its already changed
        if (_currentState == null) _currentState = _fixedName;

        _interactable = GetComponent<Interactable>();

        _SwitchState(_currentState);
    }

    [Server]
    private void Update()
    {
        if (!isServer) return;

        if(_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
        if(_cooldown <= 0)
        {
            _cooldown = 0f;
        }
    }

    public void Break()
    {
        if (brokenStateSound != null){
            FindObjectOfType<AudioManager>().PlaySound(brokenStateSound);
        }
        SwitchState(brokenState.name);
    }

    public void Fix()
    {
        if (fixedStateSound != null){
            FindObjectOfType<AudioManager>().PlaySound(fixedStateSound);
            _cooldown = brokenCooldown;
        }
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
            if (t.gameObject.name == fixedState.name || t.gameObject.name == brokenState.name)
            {
                t.gameObject.SetActive(t.gameObject.name == _currentState);
            }
        }
        _isBroken = (stateID != fixedState.name);

        if (_interactable != null) _interactable.IsInteractable = _isBroken;
    }
}
