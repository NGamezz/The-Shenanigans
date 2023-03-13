using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    JoinPlayer,
    DeviceLost,
    RegainDevice,
    StartGame,
    StartTrivia,
    Explanation
}

public static class EventManager
{
    public static Dictionary<EventType, System.Action> events = new();

    public static void AddListener(EventType type, System.Action action)
    {
        if (!events.ContainsKey(type))
        {
            events.Add(type, action);
        }
        events[type] += action;
    }

    public static void RemoveListener(EventType type, System.Action action)
    {
        if (!events.ContainsKey(type)) { return; }
        events[type] -= action;
    }

    public static void InvokeEvent(EventType type)
    {
        if (!events.ContainsKey(type)) { return; }
        events[type]?.Invoke();
    }
}
