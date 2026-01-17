using UnityEngine;

public enum EvidenceType { None, Photo, Audio, Temperature, Logs, Artifact }

[CreateAssetMenu(fileName = "NewTask", menuName = "HorrorProject/Task Data")]
public class TaskData : ScriptableObject
{
    public string taskName;
    [TextArea] public string description;
    
    [Header("Mechanics")]
    public bool isDarkTask;       // If true, increases Spirit Power
    public float duration = 3f;   // How long it takes to do
    public float spiritPowerGain = 5f; 
    
    [Header("Assignment")]
public Vector3 taskLocation; // Where employee needs to go
public EmployeeController assignedEmployee; // Set in Inspector or at runtime

    
    [Header("Rewards")]
    public EvidenceType revealsEvidence; 
}
