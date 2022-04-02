using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Use Generics of this class to support parameters
/// </summary>
public static class EventManager
{
    private static Dictionary<string, EventData> m_eventDictionary = new Dictionary<string, EventData>();

    public static void AddListener(string pEventName, EventCallback pListener)
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

    public static void RemoveListener(string pEventName, EventCallback pListener)
    {
        if (m_eventDictionary.TryGetValue(pEventName, out EventData eventData))
        {
            eventData.callbacks.Remove(pListener);
        }
    }

    public static void TriggerEvent(string pEventName)
    {
        if (m_eventDictionary.TryGetValue(pEventName, out EventData eventData))
        {
            if (eventData.isInvoking)
            {
                Debug.LogError($"Infinite recursion in EventManager on trigger event {pEventName}");
            }
            else
            {
                eventData.isInvoking = true;
                foreach (var callback in eventData.callbacks)
                {
                    callback.Invoke();
                }
            }
            eventData.isInvoking = false;
        }
    }

    private class EventData
    {
        public List<EventCallback> callbacks = new List<EventCallback>();
        public bool isInvoking;
    }
}