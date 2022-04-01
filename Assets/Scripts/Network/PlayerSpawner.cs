using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }
    public static void RemoveSpawnPoint(Transform transform)
    {
        spawnPoints.Remove(transform);
    }

    public override void OnStartServer()
    {
        CustomNetworkManager.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        CustomNetworkManager.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if(spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player at index {nextIndex}");
            return;
        }

        RpcSetPlayerPosition(conn.identity.netId, spawnPoint.position, spawnPoint.rotation);

        nextIndex++;
    }

    [ClientRpc]
    private void RpcSetPlayerPosition(uint id, Vector3 pos, Quaternion rot)
    {
        foreach (var player in CustomNetworkManager.Instance.players)
        {
            if (player.netId == id)
            {
                Debug.Log($"Set spawn position for [{id}]");
                player.transform.position = pos;
                player.yRotation = rot.eulerAngles.y;
            }
        }
    }
}
