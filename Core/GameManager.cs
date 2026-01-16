using UnityEngine;

public enum GamePhase { Working, Investigating, Convincing, Ending }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GamePhase CurrentPhase;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        
        CurrentPhase = GamePhase.Working;
    }

    private void OnEnable()
    {
        GameEvents.OnMidnightReached += HandleMidnight;
    }

    private void OnDisable()
    {
        GameEvents.OnMidnightReached -= HandleMidnight;
    }

    private void HandleMidnight()
    {
        Debug.Log("Midnight Reached - End Game Calculation");
        EndGame(false); // Default logic
    }

    public void EndGame(bool success)
    {
        CurrentPhase = GamePhase.Ending;
        Debug.Log(success ? "SURVIVED" : "CORRUPTED");
        // Load ending scene logic here
    }
}
