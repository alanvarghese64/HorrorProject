using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    [Header("Configuration")]
    [Tooltip("Drag your GameEventList ScriptableObject here")]
    public GameEventSO eventList; 

    // Dictionary to track event flags
    private Dictionary<string, bool> eventFlags = new Dictionary<string, bool>();

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

    // Initialize flags from the ScriptableObject to ensure they exist (default false)
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
            }
        }
    }

    public void SetFlag(string flagName, bool value)
    {
        if (eventFlags.ContainsKey(flagName))
        {
            eventFlags[flagName] = value;
        }
        else
        {
            eventFlags.Add(flagName, value);
        }
        Debug.Log($"Event Flag Set: {flagName} = {value}");
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
        InitializeFlags(); // Re-add the defined events
        Debug.Log("All event flags cleared and reset");
    }
}
