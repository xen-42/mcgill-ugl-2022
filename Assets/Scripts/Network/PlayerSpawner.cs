using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    private static List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();

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

    private void OnDestroy()
    {
        // Refresh spawn points list
        spawnPoints = new List<PlayerSpawnPoint>();

        CustomNetworkManager.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        PlayerSpawnPoint spawnPoint = null;

        foreach (var player in CustomNetworkManager.Instance.players)
        {
            if (player.netId == conn.identity.netId)
            {
                spawnPoint = spawnPoints.Where((x) => x.colour == player.colour).FirstOrDefault();

                if (spawnPoint == null)
                {
                    Debug.LogWarning($"Missing spawn point for player {player.colour}");
                    spawnPoint = spawnPoints.FirstOrDefault();
                    player.colour = spawnPoint.colour;
                }

                spawnPoints.Remove(spawnPoint);

                // Set up all the customization options
                spawnPoint.posters.SetSelection(player.poster);
                spawnPoint.plant.ServerSetSelection(player.plant);
                spawnPoint.drink.SetSelection(player.drink);

                HUD.Instance.RefreshPlayerIcons();
            }
        }

        RpcSpawnPlayer(conn.identity.netId, spawnPoint.transform.position, spawnPoint.transform.rotation);
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

                // If it's the local player we can fade the screen in
                if(Player.Instance == player)
                {
                    EventManager.TriggerEvent("FadeIn");
                }
            }
        }
    }
}
