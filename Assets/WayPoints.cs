using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WayPoints")]
public class WayPoints : ScriptableObject
{
    public List<Vector3> wayPoints;
    private List<Vector3>[,] grid;
    public float xLength;
    public float zLength;

    public void Gridize(float radius)
    {
        grid = new List<Vector3>[Mathf.CeilToInt(xLength / radius), Mathf.CeilToInt(zLength / radius)];    //x-z
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = new List<Vector3>();

        foreach (var pt in wayPoints)
        {
            grid[(int)(xLength / pt.x), (int)(zLength / pt.z)].Add(pt);
        }
    }

    public bool TryQueryNeighbour(Vector3 pos, ref Vector3 res)
    {
        List<Vector3> pt = Localize(pos);
        if (pt.Count == 0)
            return false;
        res = pt[Random.Range(0, pt.Count)];

        return true;
    }

    private List<Vector3> Localize(Vector3 pos) => grid[(int)(xLength / pos.x), (int)(zLength / pos.z)];
}