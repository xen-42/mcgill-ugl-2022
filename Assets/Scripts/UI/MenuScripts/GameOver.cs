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
        leftStats.SetStats(StatTracker.serverStats, StatTracker.serverUserName, StatTracker.serverSteamID);
        rightStats.SetStats(StatTracker.clientStats, StatTracker.clientUserName, StatTracker.clientSteamID);
    }

    public void OnContinueButtonPressed()
    {
        SceneManager.LoadScene(Scenes.Lobby);
    }
}
