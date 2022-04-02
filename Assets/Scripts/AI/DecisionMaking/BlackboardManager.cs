using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using static Blackboard;
using Mirror;

/// <summary>
/// Blackboard Managger.
/// Blackboard stores the global game state, where entities can read and write back
/// </summary>
public class BlackboardManager : NetworkBehaviour
{
    [SerializeField] private Blackboard blackboardInfo;

    #region Dictionaries Caches

    private static Dictionary<string, int> m_intParameters;
    private static Dictionary<string, float> m_floatParameters;
    private static Dictionary<string, bool> m_boolParameters;
    private static Dictionary<string, Trigger> m_triggerParameters;

    #endregion Dictionaries Caches

    /// <summary>
    /// Singleton
    /// </summary>

    private static BlackboardManager m_instance;

    public static BlackboardManager Instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<BlackboardManager>();

                if (!m_instance)
                {
                    Debug.LogError("There needs to be one active BlackboardManger script on a GameObject in your scene.");
                }
                else
                {
                    m_instance.Init();
                }
            }

            return m_instance;
        }
    }

    private void Awake()
    {
        if (m_instance == null)
        {
            Init();
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }

    #region Parameters Get&Set

    private void Init()
    {
        DontDestroyOnLoad(gameObject);

        m_intParameters = new Dictionary<string, int>();
        m_floatParameters = new Dictionary<string, float>();
        m_boolParameters = new Dictionary<string, bool>();
        m_triggerParameters = new Dictionary<string, Trigger>();
        blackboardInfo.LoadIntegers(m_intParameters);
        blackboardInfo.LoadFloats(m_floatParameters);
        blackboardInfo.LoadBools(m_boolParameters);
        blackboardInfo.LoadTriggers(m_triggerParameters);
    }

    public float GetFloat(string pName) => m_floatParameters[pName];

    public int GetInteger(string pName) => m_intParameters[pName];

    public bool GetBool(string pName) => m_boolParameters[pName];

    public void SetFloat(string pName, float value) => m_floatParameters[pName] = value;

    public void SetInteger(string pName, int value) => m_intParameters[pName] = value;

    public void SetBool(string pName, bool value) => m_boolParameters[pName] = value;

    public void SetTrigger(string pName) => (m_triggerParameters[pName]).Set();

    public void ResetTrigger(string pName) => (m_triggerParameters[pName]).Reset();

    public bool GetTrigger(string pName) => (m_triggerParameters[pName]).isTriggered;

    #endregion Parameters Get&Set
}