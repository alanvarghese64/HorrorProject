using UnityEngine;

public enum ActionType 
{ 
    WalkTo, 
    RunTo,
    Sit, 
    StandUp, 
    PlayAnimation, 
    Wait, 
    LookAt
}

[System.Serializable]
public class EmployeeAction
{
    [Header("Action Type")]
    public ActionType actionType;
    
[Header("Movement Settings")]
public Vector3 targetPosition; // Changed from Transform to Vector3
public Vector3 targetForward = Vector3.forward; // For chair facing direction

[Header("Sit/Stand Settings")]
public Vector3 chairPosition; // Changed from Transform
public Vector3 chairForward = Vector3.forward; // Direction to face when sitting

[Header("Look At Settings")]
public Vector3 lookAtPosition; // Changed from Transform

    
    [Header("Animation Settings")]
    public string animationTrigger = "Trigger_Work";
    public float animationDuration = 3f;
    
    [Header("Wait Settings")]
    public float waitDuration = 2f;
    
}
