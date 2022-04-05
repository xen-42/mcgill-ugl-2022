using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] StatDisplay leftStats;
    [SerializeField] StatDisplay rightStats;

    private void Start()
    {
        Debug.Log("Writing down player stats!");

        var serverStats = StatTracker.isServerLocal ? leftStats : rightStats;
        var clientStats = StatTracker.isServerLocal ? rightStats : leftStats;

        serverStats.SetStats(StatTracker.serverStats, StatTracker.serverUserName, StatTracker.serverSteamID);
        clientStats.SetStats(StatTracker.clientStats, StatTracker.clientUserName, StatTracker.clientSteamID);
    }

    public void OnContinueButtonPressed()
    {
        SceneManager.LoadScene(Scenes.Lobby);
    }
}
