using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    [Header("Configuration")]
    [Tooltip("Drag your GameEventList ScriptableObject here")]
    public GameEventSO eventList; 

    // Dictionary to track event flags
    private Dictionary<string, bool> eventFlags = new Dictionary<string, bool>();
    
    // NEW: Dictionary to track listeners for each event
    private Dictionary<string, UnityEvent> eventListeners = new Dictionary<string, UnityEvent>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFlags();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeFlags()
    {
        if (eventList != null)
        {
            foreach (var evt in eventList.eventNames)
            {
                if (!eventFlags.ContainsKey(evt))
                {
                    eventFlags.Add(evt, false);
                }
                // NEW: Initialize listener list
                if (!eventListeners.ContainsKey(evt))
                {
                    eventListeners.Add(evt, new UnityEvent());
                }
            }
        }
    }

    // MODIFIED: Now broadcasts event when flag is set
    public void SetFlag(string flagName, bool value)
    {
        if (eventFlags.ContainsKey(flagName))
        {
            eventFlags[flagName] = value;
        }
        else
        {
            eventFlags.Add(flagName, value);
            eventListeners.Add(flagName, new UnityEvent());
        }
        Debug.Log($"Event Flag Set: {flagName} = {value}");
        
        // NEW: Broadcast event if flag was set to true
        if (value && eventListeners.ContainsKey(flagName))
        {
            eventListeners[flagName].Invoke();
        }
    }

    public bool GetFlag(string flagName)
    {
        if (eventFlags.ContainsKey(flagName))
        {
            return eventFlags[flagName];
        }
        return false;
    }

    public bool HasFlag(string flagName)
    {
        return eventFlags.ContainsKey(flagName);
    }

    public void ClearFlag(string flagName)
    {
        if (eventFlags.ContainsKey(flagName))
        {
            eventFlags.Remove(flagName);
        }
    }

    public void ResetAllFlags()
    {
        eventFlags.Clear();
        eventListeners.Clear();
        InitializeFlags();
        Debug.Log("All event flags cleared and reset");
    }
    
    // NEW: Register a listener for an event
    public void AddListener(string eventName, UnityAction listener)
    {
        if (!eventListeners.ContainsKey(eventName))
        {
            eventListeners.Add(eventName, new UnityEvent());
        }
        eventListeners[eventName].AddListener(listener);
    }
    
    // NEW: Remove a listener
    public void RemoveListener(string eventName, UnityAction listener)
    {
        if (eventListeners.ContainsKey(eventName))
        {
            eventListeners[eventName].RemoveListener(listener);
        }
    }
    
    // NEW: Manually trigger an event (useful for testing)
    public void TriggerEvent(string eventName)
    {
        SetFlag(eventName, true);
    }
}
