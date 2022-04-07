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
        if (interactionEvent != null) EventManager.TriggerEvent(interactionEvent);

        if (!isServer)
        {
            Player.Instance.DoWithAuthority(netIdentity, () => CmdCompleteMinigame(Player.Instance.colour));
        }
        else
        {
            Spawn(Player.Instance.colour);
        }

        IsInteractable = true;
    }

    [Command]
    private void CmdCompleteMinigame(PlayerCustomization.COLOUR colour)
    {
        Spawn(colour);
    }

    [Server]
    private void Spawn(PlayerCustomization.COLOUR colour)
    {

        RpcPlaySound();

        var position = transform.position + Vector3.up * 0.2f;
        var newObj = Instantiate(HoldableItemPrefab, position, HoldableItemPrefab.transform.rotation);

        // Lets us track who owns it
        var holdable = newObj.GetComponent<Holdable>();
        if (holdable != null)
        {
            if (isServer)
            {
                holdable.colour = colour;
            }
            else
            {
                Player.Instance.DoWithAuthority(holdable.netIdentity, () => holdable.colour = colour);
            }
        }

        NetworkServer.Spawn(newObj);
    }

    [ClientRpc]
    public void RpcPlaySound()
    {
        if (sound != null)
        {
            sound.Play();
        }
    }
}
