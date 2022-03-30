using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Scenes
{
    private static string _mainMenu = SceneManager.GetSceneByName("MainMenu").path;
    public static string MainMenu { get => _mainMenu; }

    private static string _lobbyMenu = SceneManager.GetSceneByName("LobbyMenu").path;
    public static string Lobby { get => _lobbyMenu; }

    private static string _gameScene = SceneManager.GetSceneByName("MovementPrototype").path;
    public static string GameScene { get => _gameScene; }

    private static string _gameOver = SceneManager.GetSceneByName("GameOver").path;
    public static string GameOver { get => _gameOver; }
}
