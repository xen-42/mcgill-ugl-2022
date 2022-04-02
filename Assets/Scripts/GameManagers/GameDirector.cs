using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

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
    [SyncVar] private float _currentTime;

    private float _nextDistraction;
    private int _numDistractions;

    // Stress is out of 100
    private float _stress;
    private PostProcessingController _postProcessingController;
    public float CurrentStress => _stress;
    private bool apply_stress;

    public int NumAssignmentsDone { get; private set; }
    public int NumAssignmentsScanned { get; private set; }

    [SerializeField] public AudioSource scanSound;
    [SerializeField] public AudioSource heartbeatSound;
    [SerializeField] public AudioSource clockSound;
    private bool under30s;

    //Day -> Colour Changes
    public Light[] lightreference;
    public Light[] lightCopy;
    //Colours
    [SerializeField] public Color endColor;
    [SerializeField] public Color startingColor;
    [SerializeField] public float maxIntensity = 50f;
    [SerializeField] public float minInensityOne = 2f;
    [SerializeField] public float minInensityTwo = 20f;

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

        _postProcessingController = GameObject.Find("GlobalVolume").GetComponent<PostProcessingController>();
        _postProcessingController.DisableAllOverrides();
        apply_stress = false;
        under30s = false;

        StatTracker.Instance.RefreshStats();
    }

    public void LowerStressImmediate(float change)
    {
        _stress -= change;
        if (_stress < 0) _stress = 0;
    }

    public void DoAssignment()
    {
        NumAssignmentsDone += 1;

        StatTracker.Instance.OnSubmitAssignment();
    }

    public void ScanAssignment()
    {
        if (scanSound != null)
        {
            scanSound.Play();
        }
        NumAssignmentsScanned += 1;

        StatTracker.Instance.OnScanAssignment();
    }

    private void Update()
    {
        var available = _distractions.Where(x => x.CanBreak).ToList();
        _numDistractions = _distractions.Where(x => x.IsBroken).Count();

        if (isServer)
        {
            _currentTime += Time.deltaTime;

            _nextDistraction -= Time.deltaTime;
            if (_nextDistraction < 0)
            {
                var selection = GetRandomFromList(available);
                if (selection != null) selection.Break();
                _nextDistraction = Random.Range(minTimeBetweenDistractions, maxTimeBetweenDistractions);
            }
        }

        if (_stress > 10)
        {
            _stress += stressPerSecond * Mathf.Pow(_numDistractions, stressExponent) * Time.deltaTime * 0.5f;
        }
        else
        {
            _stress += stressPerSecond * Mathf.Pow(_numDistractions, stressExponent) * Time.deltaTime;
        }

        _stress = Mathf.Clamp(_stress, 0f, 100f);
        HUD.Instance.SetGameState(timeLimit - (int)_currentTime, _stress, NumAssignmentsDone);

        // Stress vision -----------------------------------
        // Enable stress vision
        if (_stress > 49 && _stress < 101 && !apply_stress)
        {
            apply_stress = true;
            if (heartbeatSound != null)
            {
                heartbeatSound.Play();
            }
            _postProcessingController.EnableAllOverrides();
        }
        // Disable stress vision
        if (_stress <= 49 && apply_stress)
        {
            apply_stress = false;

            heartbeatSound.volume = 0f;
            heartbeatSound.Stop();

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
            heartbeatSound.volume = (float)temp_stress * 0.02f;
            _postProcessingController.UpdateStressVision(temp_stress);
        }

        if (!under30s && _currentTime >= timeLimit - 30f)
        {
            under30s = true;
            clockSound.Play();
        }


        Player.Instance.stressModifier = Mathf.Clamp((_stress - 50f) / 50f, 0, 1);

        //Changing colour of lights

        for (int i = 0; i < lightreference.Length; i++)
     {
          lightreference[i].color = Color.Lerp(startingColor, endColor, _currentTime / timeLimit);
        }
        lightreference[0].intensity = Mathf.Lerp(minInensityTwo, maxIntensity, _currentTime / timeLimit);


        // Game Over       
        if (isServer && timeLimit <= _currentTime)
        {            
            CustomNetworkManager.Instance.Stop();
        }
    }

    private T GetRandomFromList<T>(List<T> list)
    {
        if (list.Count == 0) return default;
        return list[(int)Random.Range(0, list.Count)];
    }

    public float GetTimeLeft()
    {
        return timeLimit - _currentTime;
    }
}