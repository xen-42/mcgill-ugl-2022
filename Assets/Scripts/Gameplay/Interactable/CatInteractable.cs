using Mirror;
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

    void Start()
    {
        _unityEvent.AddListener(OnPet);
    }

    public void OnPet()
    {
        GameDirector.Instance.LowerStressImmediate(StressReduction);
        IsInteractable = false;
        if (isServer)
        {
            _cooldown = Cooldown;
        }
        else
        {
            CmdStartCooldown();
        }
    }

    private void Update()
    {
        base.Update();

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
