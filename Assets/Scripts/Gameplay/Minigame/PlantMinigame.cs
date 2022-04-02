using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantMinigame : DragDropMinigame
{
    [SerializeField] public GameObject[] plants = new GameObject[6];

    [SerializeField] public GameObject uprightWateringCan;
    [SerializeField] public GameObject pouringWateringCan;

    private int plantIndex = 0;

    [SerializeField] public float delayTime = 1f;
    private float _delay = 0f;
    private bool _completed;

    public void SetPlantSelection(PlayerCustomization.COLOUR colour, PlayerCustomization.PLANT plant)
    {
        switch(plant)
        {
            case PlayerCustomization.PLANT.CACTUS:
                plantIndex = colour == PlayerCustomization.COLOUR.COOL ? 0 : 1;
                break;
            case PlayerCustomization.PLANT.FLOWER:
                plantIndex = colour == PlayerCustomization.COLOUR.COOL ? 2 : 3;
                break;
            case PlayerCustomization.PLANT.LEAF:
                plantIndex = colour == PlayerCustomization.COLOUR.COOL ? 4 : 5;
                break;
        }

        for(int i = 0; i < plants.Length; i++)
        {
            if(i == plantIndex)
            {
                plants[i].SetActive(true);
                plants[i].transform.Find("Happy").gameObject.SetActive(false);
                plants[i].transform.Find("Sad").gameObject.SetActive(true);
            }
            else
            {
                plants[i].SetActive(false);
            }
        }
    }

    public void Update()
    {
        if (_completed)
        {
            _delay += Time.deltaTime;
            if (_delay > delayTime)
            {
                CompleteMinigame();
            }
        }
        else
        {
            // Check for completion
            if (!holdableObject.IsHeld && dropCollider.bounds.Intersects(_heldCollider.bounds))
            {
                WaterPlant();
            }
        }
    }

    public void WaterPlant()
    {
        uprightWateringCan.SetActive(false);
        pouringWateringCan.SetActive(true);

        plants[plantIndex].transform.Find("Happy").gameObject.SetActive(true);
        plants[plantIndex].transform.Find("Sad").gameObject.SetActive(false);

        _completed = true;

        StatTracker.Instance.OnWaterPlant();

        EventManager.TriggerEvent("MinigameComplete");
        OnCompleteMinigame.Invoke();
    }

    public override void CompleteMinigame()
    {
        InputManager.CurrentInputMode = InputManager.InputMode.Player;

        // Once a minigame is completed we just dispose of it
        if (gameObject != null) Destroy(gameObject);
    }
}
