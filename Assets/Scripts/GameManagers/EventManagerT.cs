using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Use Generics of this class to support parameters
/// </summary>
public static class EventManager<T>
{
    private static Dictionary<string, EventData> m_eventDictionary = new Dictionary<string, EventData>();

    public static void AddListener(string pEventName, EventCallback<T> pListener)
    {
        if (m_eventDictionary.TryGetValue(pEventName, out EventData eventData))
        {
            eventData.callbacks.Add(pListener);
        }
        else
        {
            //Add new Event
            m_eventDictionary.Add(pEventName, eventData = new EventData());
            eventData.callbacks.Add(pListener);
        }
    }

    public static void RemoveListener(string pEventName, EventCallback<T> pListener)
    {
        if (m_eventDictionary.TryGetValue(pEventName, out EventData eventData))
        {
            eventData.callbacks.Remove(pListener);
        }
    }

    public static void TriggerEvent(string pEventName, T arg)
    {
        if (m_eventDictionary.TryGetValue(pEventName, out EventData eventData))
        {
            if (eventData.isInvoking)
            {
                Debug.LogError("Infinite recursion in EventManager");
            }
            else
            {
                eventData.isInvoking = true;
                foreach (var callback in eventData.callbacks)
                {
                    callback.Invoke(arg);
                }
            }
            eventData.isInvoking = false;
        }
    }

    private class EventData
    {
        public List<EventCallback<T>> callbacks = new List<EventCallback<T>>();
        public bool isInvoking;
    }
}