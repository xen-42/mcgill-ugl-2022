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

    public bool IsBroken { get => _isBroken; }
    [SyncVar] private bool _isBroken;

    public bool CanBreak { get => (!_isBroken && _cooldown == 0f); }

    [SyncVar] private string _currentState = null;

    private Interactable _interactable;

    [SerializeField] public AudioSource brokenStateSound;
    [SerializeField] public AudioSource fixedStateSound;
    [SerializeField] public AudioSource ambientNoise;

    // Cooldown to prevent breaking right after fixing it
    [SerializeField] public float brokenCooldown = 30f;
    [SyncVar] private float _cooldown;

    [SerializeField] public float stressReduction = 20f;
    public ParticleSystem _particleSystemBroken;

    private void Awake()
    {
        _fixedName = fixedState.name;
        _brokenName = brokenState.name;

        // Since its synced you could join late and its already changed
        if (_currentState == null) _currentState = _fixedName;

        _interactable = GetComponent<Interactable>();

        _SwitchState(_currentState);

        // Have to give time for interactable to set up
        ActionManager.FireOnNextUpdate(() => _interactable.IsInteractable = IsBroken);
    }

    private void Update()
    {
        if (!isServer) return;

        // Only do the cool down when its fixed
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
        if (_cooldown <= 0)
        {
            _cooldown = 0f;
        }
    }

    public void Break()
    {
        if (brokenStateSound != null)
        {
            brokenStateSound.Play();
        }
        if (ambientNoise != null)
        {
            ambientNoise.Stop();
        }
        if (_particleSystemBroken != null){
            _particleSystemBroken.Play();
            _particleSystemBroken.loop = true;
        }

        SwitchState(brokenState.name);
    }

    public void Fix()
    {
        GameDirector.Instance.LowerStressImmediate(stressReduction);
        if (fixedStateSound != null)
        {
            fixedStateSound.Play();
        }
        if (ambientNoise != null)
        {
            ambientNoise.Play();
        }
        if (_particleSystemBroken != null){
            _particleSystemBroken.Stop();
        }
        SwitchState(fixedState.name);
    }

    private void SwitchState(string stateID)
    {
        if (!isServer)
        {
            Player.Instance.DoWithAuthority(netIdentity, () => CmdSwapState(stateID));
        }
        else
        {
            // Cooldown handled on server
            if (stateID == fixedState.name) _cooldown = brokenCooldown;

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

        _isBroken = (stateID == _brokenName);

        if (_interactable != null) _interactable.IsInteractable = _isBroken;
    }
}
