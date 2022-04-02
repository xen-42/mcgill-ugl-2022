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
        leftStats.SetStats(StatTracker.serverStats);
        rightStats.SetStats(StatTracker.clientStats);
    }

    public void OnContinueButtonPressed()
    {
        SceneManager.LoadScene(Scenes.Lobby);
    }
}
