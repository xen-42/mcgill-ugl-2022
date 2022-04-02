using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InputManager;

public class ItemSpawningMinigame : Interactable
{
    [SerializeField]
    public GameObject MinigamePrefab;

    [SerializeField]
    public GameObject HoldableItemPrefab;

    protected override InputCommand InputCommand { get => InputCommand.Interact; }
    [SerializeField] public AudioSource sound;

    void Start()
    {
        NetworkClient.RegisterPrefab(HoldableItemPrefab);

        // When the player interacts with this object it'll start the minigame
        _unityEvent.AddListener(() =>
        {
            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(this, MinigamePrefab, out var minigame);
            minigame.OnCompleteMinigame.AddListener(OnCompleteMinigame);
        });

        resetAfterUse = true;
    }

    private void OnCompleteMinigame()
    {
        if (Player.Instance.heldObject != null)
        {
            Player.Instance.CmdDrop();
        }

        // Used for tracking stats
        if(interactionEvent != null) EventManager.TriggerEvent(interactionEvent);

        if (!isServer)
        {
            Player.Instance.DoWithAuthority(netIdentity, CmdCompleteMinigame);
        }
        else
        {
            Spawn();
        }

        IsInteractable = true;
    }

    [Command]
    private void CmdCompleteMinigame()
    {
        Spawn();
    }

    [Server]
    private void Spawn()
    {
        if (sound != null){
            sound.Play();
        }
        var position = transform.position + Vector3.up * 0.2f;
        var newObj = Instantiate(HoldableItemPrefab, position, HoldableItemPrefab.transform.rotation);
        NetworkServer.Spawn(newObj);
    }
}
