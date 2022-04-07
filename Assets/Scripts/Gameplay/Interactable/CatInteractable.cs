using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InputManager;

public class CatInteractable : Interactable
{
    [SerializeField] public float Cooldown = 10f;
    [SerializeField] public int StressReduction = 20;

    [SyncVar] private float _cooldown;
    protected override InputCommand InputCommand { get => InputCommand.Interact; }

    [SerializeField] public AudioSource sound;

    void Start()
    {
        _unityEvent.AddListener(OnPet);
    }

    public void OnPet()
    {
        GameDirector.Instance.LowerStressImmediate(StressReduction);

        IsInteractable = false;

        if (interactionEvent != null)
        {
            EventManager.TriggerEvent(interactionEvent);
        }

        if (isServer)
        {
            _cooldown = Cooldown;
            RpcOnPet();
        }
        else
        {
            Player.Instance.DoWithAuthority(netIdentity,
                () =>
                {
                    CmdStartCooldown();
                    CmdOnPet();
                });
        }
    }

    [Command]
    private void CmdOnPet()
    {
        RpcOnPet();
    }

    [ClientRpc]
    private void RpcOnPet()
    {
        if (sound != null)
        {
            sound.Play();
        }
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (!IsInteractable)
        {
            _cooldown -= Time.deltaTime;
            if (_cooldown <= 0)
            {
                IsInteractable = true;
            }
        }
    }

    [Command]
    private void CmdStartCooldown()
    {
        _cooldown = Cooldown;
    }
}