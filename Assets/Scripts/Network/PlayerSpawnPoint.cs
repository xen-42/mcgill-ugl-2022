using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] public PlantInteractable plant;
    [SerializeField] public Drink drink;
    [SerializeField] public Posters posters;
    [SerializeField] public PlayerCustomization.COLOUR colour;

    void Awake()
    {
        PlayerSpawner.AddSpawnPoint(this);    
    }

    void OnDestroy()
    {
        PlayerSpawner.RemoveSpawnPoint(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}
