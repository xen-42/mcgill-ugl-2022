using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private static List<Tuple<Func<bool>, Action>> _pairs;

    private void Start()
    {
        _pairs = new List<Tuple<Func<bool>, Action>>();
    }

    /* Runs the given Action when the predicate is true */
    public static void RunWhen(Func<bool> predicate, Action action)
    {
        _pairs.Add(new Tuple<Func<bool>, Action>(predicate, action));
    }

    void Update()
    {
        var toRemove = new List<Tuple<Func<bool>, Action>>();

        /* We just check every predicate on update */
        foreach(var pair in _pairs)
        {
            if(pair.Item1.Invoke())
            {
                pair.Item2.Invoke();
                toRemove.Add(pair);
            }
        }

        foreach(var pair in toRemove)
        {
            _pairs.Remove(pair);
        }
    }
}
