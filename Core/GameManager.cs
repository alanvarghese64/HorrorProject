using UnityEngine;
using System.Collections.Generic; // ADD THIS LINE

public enum GamePhase { Working, Investigating, Convincing, Ending }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GamePhase CurrentPhase;

[Header("Employee References")]
public List<EmployeeController> allEmployees; // Assign in Inspector



    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        
        CurrentPhase = GamePhase.Working;
    }

private void Start()
{
    // Start all employees on their first task
    foreach (EmployeeController employee in allEmployees)
    {
        employee.StartNextTask();
    }
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
