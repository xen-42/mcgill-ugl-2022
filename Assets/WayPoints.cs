using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WayPoints")]
public class WayPoints : ScriptableObject
{
    public List<Vector3> wayPoints;
}