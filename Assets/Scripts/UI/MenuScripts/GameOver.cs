using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] StatDisplay leftStats;
    [SerializeField] StatDisplay rightStats;

    [SerializeField] Image leftWinImage;
    [SerializeField] Image rightWinImage;

    private void Start()
    {
        Debug.Log("Writing down player stats!");

        var serverStats = StatTracker.isServerLocal ? leftStats : rightStats;
        var clientStats = StatTracker.isServerLocal ? rightStats : leftStats;

        serverStats.SetStats(StatTracker.serverStats, StatTracker.serverUserName, StatTracker.serverSteamID);
        clientStats.SetStats(StatTracker.clientStats, StatTracker.clientUserName, StatTracker.clientSteamID);

        DetermineWinner();
    }

    public void OnContinueButtonPressed()
    {
        SceneManager.LoadScene(Scenes.Lobby);
    }

    void DetermineWinner()
    {
        var leftGrade = leftStats.letterGradeIndex;
        var rightGrade = rightStats.letterGradeIndex;

        if (leftGrade > rightGrade)
        {
            // Left wins
            leftWinImage.enabled = true;
            rightWinImage.enabled = false;
        }
        else if (rightGrade > leftGrade)
        {
            // Right wins
            leftWinImage.enabled = false;
            rightWinImage.enabled = true;
        }
        else
        {
            leftWinImage.enabled = true;
            rightWinImage.enabled = true;
        }
    }
}
