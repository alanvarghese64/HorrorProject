using UnityEngine;

public class SpiritManager : MonoBehaviour
{
    public static SpiritManager Instance;

    [Header("Status")]
    public float currentSpiritPower = 0f;
    public float corruptionThreshold = 100f;

    private void Awake()
    {
        Instance = this;
    }

    public void AddPower(float amount)
    {
        currentSpiritPower += amount;
        GameEvents.OnSpiritPowerChanged?.Invoke(currentSpiritPower);

        if (currentSpiritPower >= corruptionThreshold)
        {
            GameEvents.OnCorruptionThresholdReached?.Invoke();
        }
        
        Debug.Log($"Spirit Power: {currentSpiritPower}");
    }
}
