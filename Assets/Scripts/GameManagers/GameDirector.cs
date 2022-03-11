using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDirector : NetworkBehaviour
{
    public static GameDirector Instance;

    private List<Fixable> _distractions;

    [SerializeField] public int timeLimit;

    public float Countdown { get; private set; }

    private float _nextDistraction;

    private int _numDistractions;

    private float _stress;

    private int _assignments;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        if (!isServer) return;

        _distractions = FindObjectsOfType<Fixable>().ToList();
        Random.InitState((int)System.DateTime.Now.Ticks);

        _nextDistraction = 10f;

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
        _assignments += 1;
    }

    [Server]
    private void Update()
    {
        if (!isServer) return;

        Countdown += Time.deltaTime;
        _nextDistraction -= Time.deltaTime;

        var available = _distractions.Where(x => !x.IsBroken()).ToList();
        _numDistractions = _distractions.Count - available.Count;

        if (_nextDistraction < 0)
        {
            Debug.Log("Distraction!");

            var selection = GetRandomFromList(available);
            if(selection != null) selection.Break();
            _nextDistraction = Random.Range(5, 10);
        }

        _stress += _numDistractions * _numDistractions * Time.deltaTime;

        HUD.Instance.SetGameState(timeLimit - (int)Countdown, (int)_stress, _assignments);
    }

    private T GetRandomFromList<T>(List<T> list)
    {
        if (list.Count == 0) return default;
        return list[(int)Random.Range(0, list.Count)];
    }
}
