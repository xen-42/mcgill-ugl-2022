using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPointsLoader : MonoBehaviour
{
    public WayPoints spawnPointsContainer;
    public bool run;

    private void Update()
    {
        if (run && spawnPointsContainer != null)
        {
            foreach (Transform ts in transform)
            {
                foreach (Transform tts in ts)
                {
                    spawnPointsContainer.wayPoints.Add(tts.position);
                }
            }
            run = false;
        }
    }
}