using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance; // ADD THIS
    
    [Header("Settings")]
    [Range(0, 24)] public float currentTime = 9f;
    public float timeScale = 1f;
    
    private bool midnightTriggered = false;
    
    private void Awake() // ADD THIS METHOD
    {
        if (Instance == null) Instance = this;
    }


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
