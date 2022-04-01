using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    private static List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Add(spawnPoint);

        spawnPoints = spawnPoints.OrderBy(x => x.transform.GetSiblingIndex()).ToList();
    }
    public static void RemoveSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Remove(spawnPoint);
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
        PlayerSpawnPoint spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if(spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player at index {nextIndex}");
            return;
        }

        RpcSpawnPlayer(conn.identity.netId, spawnPoint.transform.position, spawnPoint.transform.rotation);

        foreach (var player in CustomNetworkManager.Instance.players)
        {
            if (player.netId == conn.identity.netId)
            {
                spawnPoint.posters.SetSelection(player.poster);
                spawnPoint.plant.SetSelection(player.plant);
                spawnPoint.drink.SetSelection(player.drink);
            }
        }



        nextIndex++;
    }

    [ClientRpc]
    private void RpcSpawnPlayer(uint id, Vector3 pos, Quaternion rot)
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
