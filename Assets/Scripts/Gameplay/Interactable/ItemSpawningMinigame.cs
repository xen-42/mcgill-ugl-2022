using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemSpawningMinigame : Interactable
{
    [SerializeField]
    public GameObject MinigamePrefab;

    [SerializeField]
    public GameObject HoldableItemPrefab;

    void Start()
    {
        NetworkClient.RegisterPrefab(HoldableItemPrefab);

        // When the player interacts with this object it'll start the minigame
        _unityEvent.AddListener(() =>
        {
            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(MinigamePrefab, out var minigame);
            minigame.OnCompleteMinigame.AddListener(OnCompleteMinigame);
        });
    }

    private void OnCompleteMinigame()
    {
        if (Player.Instance.heldObject != null)
        {
            Player.Instance.CmdDrop();
        }

        if (!isServer)
        {
            // Need authority before we can give commands
            Player.Instance.CmdGiveAuthority(netIdentity);
            ActionManager.RunWhen(() => netIdentity.hasAuthority, CmdCompleteMinigame);
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
        var position = transform.position + Vector3.up * 0.2f;
        var newObj = Instantiate(HoldableItemPrefab, position, HoldableItemPrefab.transform.rotation);
        NetworkServer.Spawn(newObj);
    }
}
