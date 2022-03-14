using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDirector : NetworkBehaviour
{
    // Exposed modifiers for game balancing
    [SerializeField] public int timeLimit;
    [SerializeField] public float minTimeBetweenDistractions;
    [SerializeField] public float maxTimeBetweenDistractions;
    [SerializeField] public float timeUntilFirstDistraction;

    // How much stress is gained per second for one distraction
    [SerializeField] public float stressPerSecond;

    // Stress gain is proportional to the number of distractions to this power
    [SerializeField] public float stressExponent;

    [SerializeField] public int assignmentsGoal;

    // This is a singleton class
    public static GameDirector Instance;

    private List<Fixable> _distractions;

    // Host controls the timer
    [SyncVar] private float _countdown;

    private float _nextDistraction;

    private int _numDistractions;

    private float _stress;

    public int NumAssignmentsDone { get; private set; }
    public int NumAssignmentsScanned { get; private set; }

    private bool _gameOver;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        _distractions = FindObjectsOfType<Fixable>().ToList();
        Random.InitState((int)DateTime.Now.Ticks);

        _nextDistraction = timeUntilFirstDistraction;

        EventManager<int>.AddListener("LowerStress", LowerStress);
    }

    private void OnDestroy()
    {
        EventManager<int>.RemoveListener("LowerStress", LowerStress);
    }

    public void LowerStress(int change)
    {
        _stress -= change;
        if (_stress < 0) _stress = 0;
    }

    public void DoAssignment()
    {
        NumAssignmentsDone += 1;
    }

    public void ScanAssignment()
    {
        NumAssignmentsScanned += 1;
    }

    private void Update()
    {
        if (_gameOver) return;

        var available = _distractions.Where(x => !x.IsBroken()).ToList();
        _numDistractions = _distractions.Count - available.Count;

        if (isServer)
        {
            _countdown += Time.deltaTime;

            _nextDistraction -= Time.deltaTime;
            if (_nextDistraction < 0)
            {
                var selection = GetRandomFromList(available);
                if (selection != null) selection.Break();
                _nextDistraction = Random.Range(minTimeBetweenDistractions, maxTimeBetweenDistractions);
            }
        }

        _stress += stressPerSecond * Mathf.Pow(_numDistractions, stressExponent) * Time.deltaTime;

        HUD.Instance.SetGameState(timeLimit - (int)_countdown, (int)_stress, NumAssignmentsDone);

        // Game Over       
        if (!_gameOver && timeLimit == _countdown)
        {
            _gameOver = true;
            InputManager.CurrentInputMode = InputManager.InputMode.UI;
        }
    }

    private T GetRandomFromList<T>(List<T> list)
    {
        if (list.Count == 0) return default;
        return list[(int)Random.Range(0, list.Count)];
    }
}
