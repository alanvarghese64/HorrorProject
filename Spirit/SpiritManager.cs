using UnityEngine;

public class SpiritManager : MonoBehaviour
{
    public static SpiritManager Instance;
    
    [Header("Spirit Power")]
    public float spiritPower = 0f;
    public float manifestationThreshold = 20f;
    public float corruptionThreshold = 50f;
    
    private bool hasManifested = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    
    private void OnEnable()
    {
        GameEvents.OnDarkTaskStarted += OnDarkTaskCompleted;
    }
    
    private void OnDisable()
    {
        GameEvents.OnDarkTaskStarted -= OnDarkTaskCompleted;
    }
    
    private void OnDarkTaskCompleted(TaskData task)
    {
        AddPower(task.spiritPowerGain);
    }

    public void AddPower(float amount)
    {
        spiritPower += amount;
        GameEvents.OnSpiritPowerChanged?.Invoke(spiritPower);
        
        Debug.Log($"Spirit Power: {spiritPower}");
        
        if (spiritPower >= manifestationThreshold && !hasManifested)
        {
            hasManifested = true;
            StartManifestations();
        }
        
        if (spiritPower >= corruptionThreshold)
        {
            GameEvents.OnCorruptionThresholdReached?.Invoke();
        }
    }
    
    private void StartManifestations()
    {
        Debug.Log("Spirit begins manifesting...");
        // Trigger environmental effects
        // Change lighting, play sounds, etc.
    }
}
