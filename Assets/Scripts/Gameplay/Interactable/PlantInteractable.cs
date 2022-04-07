using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantInteractable : MinigameInteractable
{
    [SyncVar] [SerializeField] public PlayerCustomization.COLOUR colour;
    [SyncVar] [SerializeField] public PlayerCustomization.PLANT plant;

    [SerializeField] public GameObject[] cactusObjects;
    [SerializeField] public GameObject[] leafObjects;
    [SerializeField] public GameObject[] flowerObjects;

    // Start is called before the first frame update
    void Start()
    {
        // When the player interacts with this object it'll start the minigame
        _unityEvent.AddListener(() =>
        {
            IsInteractable = false;
            MinigameManager.Instance.StartMinigame(this, MinigamePrefab, out var minigame);
            minigame.OnCompleteMinigame.AddListener(() => OnCompleteMinigame.Invoke());
            minigame.OnCompleteMinigame.AddListener(() => IsInteractable = true);

            // TODO: customization thing
            (minigame as PlantMinigame).SetPlantSelection(colour, plant);

            if (requiredObject != Holdable.Type.NONE)
            {
                // Should never be null but could be a bug and it'll hang the player
                if (Player.Instance.heldObject != null)
                {
                    // Try consuming the required object after use
                    minigame.OnCompleteMinigame.AddListener(() => Player.Instance.heldObject.Consume());
                }
            }
        });

        SetSelection(plant);
    }

    [Server]
    public void ServerSetSelection(PlayerCustomization.PLANT selection)
    {
        plant = selection;
        RpcSetSelection(selection);
    }

    [ClientRpc]
    public void RpcSetSelection(PlayerCustomization.PLANT selection)
    {
        SetSelection(selection);
    }

    private void SetSelection(PlayerCustomization.PLANT selection)
    {
        foreach (var cactus in cactusObjects)
        {
            cactus.SetActive(selection == PlayerCustomization.PLANT.CACTUS);
        }
        foreach (var leaf in leafObjects)
        {
            leaf.SetActive(selection == PlayerCustomization.PLANT.LEAF);
        }
        foreach (var flower in flowerObjects)
        {
            flower.SetActive(selection == PlayerCustomization.PLANT.FLOWER);
        }
    }
}
