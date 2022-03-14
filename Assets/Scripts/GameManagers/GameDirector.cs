using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDirector : NetworkBehaviour
{
    public static GameDirector Instance;

    private List<Fixable> _distractions;

    #region Time Countdown-Related Variables

    [SerializeField] public int timeLimit;

    // Host controls the timer
    [SyncVar] private float _countdown;

    #endregion Time Countdown-Related Variables

    #region Distraction-Related Variables

    private float _nextDistraction;

    private int _numDistractions;

    #endregion Distraction-Related Variables

    #region Stress-Related Variables

    private float _stress;
    [SyncVar] [SerializeField] private float _maxStress;
    private bool _isStressDecreasing;
    [SerializeField] private float _stressDecreasingTime = .5f;

    public float MaxStress => _maxStress;
    public float CurrentStress => _stress;

    #endregion Stress-Related Variables

    #region Assignment-Related Variables

    public int NumAssignmentsDone { get; private set; }
    public int NumAssignmentsScanned { get; private set; }

    #endregion Assignment-Related Variables

    #region Game State Variables

    private bool _gameOver;

    #endregion Game State Variables

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Instance = this;

        _distractions = FindObjectsOfType<Fixable>().ToList();
        Random.InitState((int)System.DateTime.Now.Ticks);

        _nextDistraction = 10f;

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
                Debug.Log("Distraction!");

                var selection = GetRandomFromList(available);
                if (selection != null) selection.Break();
                _nextDistraction = Random.Range(5, 10);
            }
        }

        if (!_isStressDecreasing)
            _stress += _numDistractions * _numDistractions * Time.deltaTime;

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