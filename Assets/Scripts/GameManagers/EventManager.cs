using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Use Generics of this class to support paramters
/// </summary>
public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> m_eventDictionary;
    private static EventManager m_manager;

    public static EventManager Instance
    {
        get
        {
            if (!m_manager)
            {
                m_manager = FindObjectOfType<EventManager>();
                if (!m_manager)
                {
                    Debug.LogError("There needs to be one active Event Manager script on a GameObject in your scene.");
                }
                else
                {
                    m_manager.Init();
                }
            }

            return m_manager;
        }
    }

    private void Init()
    {
        //Init Dictionary
        m_eventDictionary = new Dictionary<string, UnityEvent>();
    }

    public void AddListener(string pEventName, UnityAction pListener)
    {
        if (m_eventDictionary.TryGetValue(pEventName, out UnityEvent eve))
            eve.AddListener(pListener);
        else
        {
            //Add new Event
            m_eventDictionary.Add(pEventName, (eve = new UnityEvent()));
            eve.AddListener(pListener);
        }
    }

    public void RemoveListener(string pEventName, UnityAction pListener)
    {
        if (m_eventDictionary.TryGetValue(pEventName, out UnityEvent eve))
            eve.RemoveListener(pListener);
    }

    public void TriggerEvent(string pEventName)
    {
        if (m_eventDictionary.TryGetValue(pEventName, out UnityEvent eve))
            eve.Invoke();
    }
}