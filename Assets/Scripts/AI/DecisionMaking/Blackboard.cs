using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Blackboard is the place to store global game states.
/// The names of same type of parameters can not repeat. Duplicate Names will be automatically handled by the class.
/// </summary>
[CreateAssetMenu(fileName = "BlackBoard")]
public class Blackboard : ScriptableObject
{
    [Serializable]
    public class Trigger
    {
        [Tooltip("Is it triggered or not")]
        public bool isTriggered;

        public void Set() => isTriggered = true;

        public void Reset() => isTriggered = false;
    }

    [Serializable]
    public struct BlackboardParam<T>
    {
        [Tooltip("The name of the parameter.")]
        public string name;

        [Tooltip("The initial value of the parameter.")]
        public T initValue;
    }

    [Header("BlackBoard")]
    [Space(10)]
    public BlackboardParam<int>[] IntegerParams;
    public BlackboardParam<float>[] FloatParams;
    public BlackboardParam<bool>[] BoolParams;
    public BlackboardParam<Trigger>[] TriggerParams;

    /// <summary>
    /// Load Intergers into the table
    /// </summary>
    /// <param name="table"></param>
    public void LoadIntegers(Dictionary<string, int> table)
    {
        //Load Integers
        for (int i = 0; i < IntegerParams.Length; i++)
        {
            //Check Name Duplicity
            IntegerParams[i].name = AdjustName(table, IntegerParams[i].name);
            table.Add(IntegerParams[i].name, IntegerParams[i].initValue);
        }
    }

    /// <summary>
    /// Load Floats into the table
    /// </summary>
    /// <param name="table"></param>
    public void LoadFloats(Dictionary<string, float> table)
    {
        //Load Floats
        for (int i = 0; i < FloatParams.Length; i++)
        {
            //Check Name Duplicity
            FloatParams[i].name = AdjustName(table, FloatParams[i].name);
            table.Add(FloatParams[i].name, FloatParams[i].initValue);
        }
    }

    /// <summary>
    /// Load Bools into the table
    /// </summary>
    /// <param name="table"></param>
    public void LoadBools(Dictionary<string, bool> table)
    {
        //Load Bools
        for (int i = 0; i < BoolParams.Length; i++)
        {
            //Check Name Duplicity
            BoolParams[i].name = AdjustName(table, BoolParams[i].name);
            table.Add(BoolParams[i].name, BoolParams[i].initValue);
        }
    }

    /// <summary>
    /// Load Triggers into the table
    /// </summary>
    /// <param name="table"></param>
    public void LoadTriggers(Dictionary<string, Trigger> table)
    {
        //Load Triggers
        for (int i = 0; i < TriggerParams.Length; i++)
        {
            //Check Name Duplicity
            TriggerParams[i].name = AdjustName(table, TriggerParams[i].name);
            table.Add(TriggerParams[i].name, TriggerParams[i].initValue);
        }
    }

    private string AdjustName<T>(Dictionary<string, T> table, string originalName)
    {
        if (!table.ContainsKey(originalName))
            return originalName;

        int id = 0;
        string newName;
        do newName = originalName + '(' + (++id).ToString() + ')';
        while (table.ContainsKey(newName));

        //Table doesn't contain the new name
        return newName;
    }
}