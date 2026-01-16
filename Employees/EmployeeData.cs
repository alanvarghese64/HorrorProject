using UnityEngine;

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
}
