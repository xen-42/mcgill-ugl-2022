using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Scenes
{
    public static int OpeningScene { get => 0; }
    public static int MainMenu { get => 1; }
    public static int GameScene { get => 2; }
    public static int Lobby { get => 3; }
    public static int GameOver { get => 4; }
    public static int Credits { get => 5; }
    public static int Tutorial { get => 6; }
}
