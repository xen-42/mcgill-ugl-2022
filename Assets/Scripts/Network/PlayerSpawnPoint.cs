using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] public GameObject plant;
    [SerializeField] public GameObject drink;
    [SerializeField] public Posters posters;

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
