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
    [SyncVar] [SerializeField] private float _maxStress;
    private bool _isStressDecreasing;
    [SerializeField] private float _stressDecreasingTime = .5f;

    public float MaxStress => _maxStress;
    public float CurrentStress => _stress;

    public int NumAssignmentsDone { get; private set; }
    public int NumAssignmentsScanned { get; private set; }

    private bool _gameOver;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Instance = this;

        _distractions = FindObjectsOfType<Fixable>().ToList();
        Random.InitState((int)DateTime.Now.Ticks);

        _nextDistraction = timeUntilFirstDistraction;

        EventManager<float>.AddListener("LowerStress", LowerStress);
    }

    private void OnDestroy()
    {
        EventManager<float>.RemoveListener("LowerStress", LowerStress);
    }

    private void LowerStress(float change)
    {
        //_stress -= change;
        //if (_stress < 0) _stress = 0;
        StartCoroutine(nameof(StressDecreasing), change);
    }

    private IEnumerable StressDecreasing(float change)
    {
        float timeElapsed = 0f;
        float tgtStressVal = Mathf.Max(_stress - change, 0f);
        _isStressDecreasing = true;

        while (timeElapsed < _stressDecreasingTime)
        {
            timeElapsed += Time.deltaTime;
            _stress = Mathf.Lerp(_stress, tgtStressVal, timeElapsed / _stressDecreasingTime);
            HUD.Instance.SetStressValue(_stress);
            yield return null;
        }

        _stress = tgtStressVal;
        _isStressDecreasing = false;
        yield return null;
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

        var available = _distractions.Where(x => !x.IsBroken).ToList();
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

        if (!_isStressDecreasing)
			_stress += stressPerSecond * Mathf.Pow(_numDistractions, stressExponent) * Time.deltaTime;

        _stress = Mathf.Clamp(_stress, 0f, 100f);
        HUD.Instance.SetGameState(timeLimit - (int)_countdown, _stress, NumAssignmentsDone);

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