using UnityEngine;
using System.Collections.Generic; // ADD THIS AT TOP

public enum EvidenceType { None, Photo, Audio, Temperature, Logs, Artifact }

[CreateAssetMenu(fileName = "NewTask", menuName = "HorrorProject/Task Data")]
public class TaskData : ScriptableObject
{
    public string taskName;
    [TextArea] public string description;
    
[Header("Mechanics")]
public bool isDarkTask;       
public float spiritPowerGain = 5f; 

[Header("Action Sequence")]
public List<EmployeeAction> actionSequence = new List<EmployeeAction>();

[Header("Rewards")]
public EvidenceType revealsEvidence;
}
