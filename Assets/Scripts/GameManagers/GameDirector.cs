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

    // Stress is out of 100
    private float _stress;
    private bool _isStressDecreasing;
    [SerializeField] private float _stressDecreasingTime = 0.5f;
    private bool _stressed_out;
    private PostProcessingController _postProcessingController;
    public float CurrentStress => _stress;
    private bool apply_stress;

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
        _distractions = FindObjectsOfType<Fixable>().ToList();
        Random.InitState((int)DateTime.Now.Ticks);

        _nextDistraction = timeUntilFirstDistraction;

        _stressed_out = false;
        _postProcessingController = GameObject.Find("GlobalVolume").GetComponent<PostProcessingController>();

        _postProcessingController.DisableAllOverrides();
        apply_stress = false;
    }

    public void LowerStressImmediate(float change)
    {
        _stress -= change;
        if (_stress < 0) _stress = 0;
    }

    public void LowerStressGradually(float change)
    {
        StartCoroutine(nameof(StressDecreasing), change);
    }

    private IEnumerator StressDecreasing(float change)
    {
        float timeElapsed = 0f;
        float targetStressValue = Mathf.Max(_stress - change, 0f);
        _isStressDecreasing = true;

        while (timeElapsed < _stressDecreasingTime)
        {
            timeElapsed += Time.deltaTime;
            _stress = Mathf.Lerp(_stress, targetStressValue, timeElapsed / _stressDecreasingTime);
            HUD.Instance.SetStressValue(_stress);

            if (_stress > 49)
            {
                _postProcessingController.UpdateStressVision(_stress - 50);
            }

            yield return null;
        }

        _stress = targetStressValue;
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
        {
            if (_stress > 10)
            {
                _stress += stressPerSecond * Mathf.Pow(_numDistractions, stressExponent) * Time.deltaTime * 0.5f;
            }
            else
            {
                _stress += stressPerSecond * Mathf.Pow(_numDistractions, stressExponent) * Time.deltaTime;
            }
        }

        _stress = Mathf.Clamp(_stress, 0f, 100f);
        HUD.Instance.SetGameState(timeLimit - (int)_countdown, _stress, NumAssignmentsDone);

        // Stress vision -----------------------------------
        // Enable stress vision
        if (_stress > 49 && _stress < 101 && !apply_stress)
        {
            apply_stress = true;
            _postProcessingController.EnableAllOverrides();
        }
        // Disable stress vision
        if (_stress <= 49 && apply_stress)
        {
            apply_stress = false;
            _postProcessingController.DisableAllOverrides();
            Player.Instance.moveSpeed = 6f;
            Player.Instance.walkSpeed = 4f;
            Player.Instance.runSpeed = 6f;
            Player.Instance.acceleration = 10f;
        }
        // Apply modifications
        if (apply_stress)
        {
            float temp_stress = _stress - 50;
            _postProcessingController.UpdateStressVision(temp_stress);
        }

        Player.Instance.stressModifier = Mathf.Clamp((_stress - 50f) / 50f, 0, 1);

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