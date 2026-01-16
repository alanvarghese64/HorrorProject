using System;
using UnityEngine;

public static class GameEvents
{
    // Task Events
    public static Action<TaskData> OnTaskCompleted;
    public static Action<TaskData> OnDarkTaskStarted;

    // Spirit/Horror Events
    public static Action<float> OnSpiritPowerChanged; // float = current total power
    public static Action OnCorruptionThresholdReached;

    // Employee Events
    public static Action<EmployeeController> OnEmployeeConvinced;
    public static Action<EmployeeController> OnEmployeePossessed;

    // Game Flow
    public static Action OnMidnightReached;
}
