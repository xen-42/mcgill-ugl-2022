using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private static List<Tuple<Func<bool>, Action>> _pairs;
    private static List<Action> _nextTick;

    private static bool initialized = false;

    private void Start()
    {
        if (!initialized) Init();
    }

    static void Init()
    {
        _pairs = new List<Tuple<Func<bool>, Action>>();
        _nextTick = new List<Action>();
        initialized = true;
    }

    /* Runs the given Action when the predicate is true */
    public static void RunWhen(Func<bool> predicate, Action action)
    {
        if (!initialized) Init();
        _pairs.Add(new Tuple<Func<bool>, Action>(predicate, action));
    }

    public static void FireOnNextUpdate(Action action)
    {
        if (!initialized) Init();
        _nextTick.Add(action);
    }

    void Update()
    {
        var toRemove = new List<Tuple<Func<bool>, Action>>();

        /* We just check every predicate on update */
        foreach (var pair in _pairs)
        {
            if (pair.Item1.Invoke())
            {
                pair.Item2.Invoke();
                toRemove.Add(pair);
            }
        }

        foreach (var pair in toRemove)
        {
            _pairs.Remove(pair);
        }

        foreach (var action in _nextTick)
        {
            action.Invoke();
        }
    }
}
