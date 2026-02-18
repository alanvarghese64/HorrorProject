using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Settings")]
    public float aiReactivationDelay = 2f; // Seconds after UI closes
    
    private bool isAnyUIActive = false;
    private int activeUICount = 0; // Track multiple UIs
    
    // Event system for AI to listen to
    public delegate void UIStateChanged(bool isActive);
    public static event UIStateChanged OnUIStateChanged;
    
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
    
    // Call this when ANY UI opens
    public void RegisterUIOpen()
    {
        activeUICount++;
        
        if (!isAnyUIActive)
        {
            isAnyUIActive = true;
            OnUIStateChanged?.Invoke(true); // Notify AI immediately
            Debug.Log("UI Opened - Ghost AI paused");
        }
    }
    
    // Call this when ANY UI closes
    public void RegisterUIClose()
    {
        activeUICount--;
        
        if (activeUICount <= 0)
        {
            activeUICount = 0;
            
            // Delay reactivation
            StartCoroutine(DelayedUIClose());
        }
    }
    
    IEnumerator DelayedUIClose()
    {
        yield return new WaitForSeconds(aiReactivationDelay);
        
        // Double check no UI opened during delay
        if (activeUICount == 0)
        {
            isAnyUIActive = false;
            OnUIStateChanged?.Invoke(false); // Notify AI to resume
            Debug.Log($"UI Closed - Ghost AI reactivating in {aiReactivationDelay}s");
        }
    }
    
    public bool IsUIActive()
    {
        return isAnyUIActive;
    }
    
    // Force immediate reactivation (emergency use)
    public void ForceCloseAll()
    {
        activeUICount = 0;
        isAnyUIActive = false;
        OnUIStateChanged?.Invoke(false);
    }
}
