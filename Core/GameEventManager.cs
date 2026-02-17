using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    // Dictionary to track event flags
    private Dictionary<string, bool> eventFlags = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Set an event flag
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

    // Get an event flag (returns false if doesn't exist)
    public bool GetFlag(string flagName)
    {
        if (eventFlags.ContainsKey(flagName))
        {
            return eventFlags[flagName];
        }
        return false;
    }

    // Check if flag exists
    public bool HasFlag(string flagName)
    {
        return eventFlags.ContainsKey(flagName);
    }

    // Clear a flag
    public void ClearFlag(string flagName)
    {
        if (eventFlags.ContainsKey(flagName))
        {
            eventFlags.Remove(flagName);
            Debug.Log($"Event Flag Cleared: {flagName}");
        }
    }

    // Reset all flags (useful for new game)
    public void ResetAllFlags()
    {
        eventFlags.Clear();
        Debug.Log("All event flags cleared");
    }
}
