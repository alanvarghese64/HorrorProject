using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(0, 24)] public float currentTime = 9f; // Start at 9 AM
    public float timeScale = 1f; // 1 real sec = 1 game minute? Adjust as needed.
    
    private bool midnightTriggered = false;

    void Update()
    {
        if (GameManager.Instance.CurrentPhase == GamePhase.Ending) return;

        currentTime += Time.deltaTime * timeScale;

        if (currentTime >= 24f && !midnightTriggered)
        {
            midnightTriggered = true;
            GameEvents.OnMidnightReached?.Invoke();
        }
    }
}
