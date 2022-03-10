using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDirector : NetworkBehaviour
{
    public static GameDirector Instance;

    [SerializeField]
    private List<Fixable> _distractions;

    public float Countdown { get; private set; }

    private float _nextDistraction;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        if (!isServer) return;

        _distractions = FindObjectsOfType<Fixable>().ToList();
        Random.InitState((int)System.DateTime.Now.Ticks);

        _nextDistraction = 10f;
    }

    [Server]
    private void Update()
    {
        if (!isServer) return;

        Countdown += Time.deltaTime;
        _nextDistraction -= Time.deltaTime;

        if(_nextDistraction < 0)
        {
            Debug.Log("Distraction!");
            var selection = GetRandomFromList(_distractions.Where(x => !x.IsBroken()).ToList());
            if(selection != null) selection.Break();
            _nextDistraction = Random.Range(5, 10);
        }
    }

    private T GetRandomFromList<T>(List<T> list)
    {
        if (list.Count == 0) return default;
        return list[(int)Random.Range(0, list.Count)];
    }
}
