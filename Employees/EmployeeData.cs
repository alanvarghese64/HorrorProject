using UnityEngine;
using System.Collections.Generic;

public enum BeliefType { Rational, Superstitious, Visual, Authority, Experiential, Skeptic }

[CreateAssetMenu(fileName = "NewEmployee", menuName = "HorrorProject/Employee Data")]
public class EmployeeData : ScriptableObject
{
    public string employeeName;
    public BeliefType beliefType;

    public ReactionProfile reactionProfile;
    [Range(0, 100)] public float startingResistance = 50f;
    
    [Header("Dialogues")]
    [TextArea] public string introDialogue;
    [TextArea] public string convincedDialogue;
    
    [Header("Task Assignment")]
    public List<TaskData> assignedTaskSequence;
    public float timeBetweenTasks = 30f;
    
    [Header("Conviction System")]  // ADD THIS ENTIRE SECTION
    public List<EvidenceType> preferredEvidenceTypes;
    public float currentConviction = 0f;
    public float convictionThreshold = 100f;
}
