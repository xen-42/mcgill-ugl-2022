using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsManager : MonoBehaviour
{
    public WayPoints spawnPoints;

    public Vector3 pivot;
    public float xLength;
    public float zLength;
    public float radius;

    private List<Vector3>[,] grid;

    private List<Vector3> Localize(Vector3 pos) => grid[(int)((pos.x - pivot.x) / radius), (int)((pos.z - pivot.z) / radius)];

    private void Awake()
    {
        grid = new List<Vector3>[Mathf.CeilToInt(xLength / radius), Mathf.CeilToInt(zLength / radius)];    //x-z
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = new List<Vector3>();

        foreach (var pt in spawnPoints.wayPoints)
        {
            Localize(pt).Add(pt);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public bool TryQueryNeighbour(Vector3 pos, out Vector3 res)
    {
        List<Vector3> pt = Localize(pos);
        if (pt.Count == 0)
        {
            res = default;
            return false;
        }
        res = pt[Random.Range(0, pt.Count)];

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            //Draw Grids
            //grid = new List<Vector3>[Mathf.CeilToInt(xLength / radius), Mathf.CeilToInt(zLength / radius)];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                Vector3 zOffset = Vector3.forward * i * radius;
                Vector3 bottomLeftBase = pivot + zOffset;
                Vector3 bottomLeft;
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    bottomLeft = bottomLeftBase + Vector3.right * j * radius;
                    DrawGrid(bottomLeft);
                }
            }
        }
        void DrawGrid(Vector3 bottomLeft)
        {
            print("Hello");
            Vector3 btmRight = bottomLeft + Vector3.right * radius;
            Vector3 topLeft = bottomLeft + Vector3.forward * radius;
            Vector3 topRight = topLeft + Vector3.right * radius;
            Gizmos.DrawLine(bottomLeft, btmRight);
            Gizmos.DrawLine(btmRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }
}