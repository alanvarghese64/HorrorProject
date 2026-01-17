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
    public Transform targetLocation;
    
    [Header("Sit/Stand Settings")]
    public Transform chairTransform;
    
    [Header("Animation Settings")]
    public string animationTrigger = "Trigger_Work";
    public float animationDuration = 3f;
    
    [Header("Wait Settings")]
    public float waitDuration = 2f;
    
    [Header("Look At Settings")]
    public Transform lookAtTarget;
}
