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
        ActionManager.RunWhen(() => StatTracker.Instance != null, () =>
        {
            Debug.Log("Writing down player stats!");
            leftStats.SetStats(StatTracker.Instance.serverStats);
            rightStats.SetStats(StatTracker.Instance.clientStats);
        });
    }

    public void OnContinueButtonPressed()
    {
        SceneManager.LoadScene(Scenes.Lobby);
    }
}
