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

    #region Caches

    private CatAgent m_cat;

    #endregion Caches

    protected override void Awake()
    {
        base.Awake();
        m_cat = GetComponent<CatAgent>();
    }

    [SerializeField] public string sound;

    void Start()
    {
        _unityEvent.AddListener(OnPet);
    }

    public void OnPet()
    {
        if (sound != null){
            FindObjectOfType<AudioManager>().PlaySound(sound);
        }
        GameDirector.Instance.LowerStressImmediate(StressReduction);
        m_cat.OnUpdatePetStatus(Player.Instance);

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

        Debug.Log("CatInteractable" + InteractablePrompt.Text);
    }

    [Command]
    private void CmdStartCooldown()
    {
        _cooldown = Cooldown;
    }
}