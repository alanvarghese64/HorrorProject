using System.Collections;
using UnityEngine;

public class TaskInstance : MonoBehaviour
{
    public TaskData data;
    private bool isBusy = false;

    // Called by Player or Employee
    public void Execute(EmployeeController executor = null)
    {
        if (isBusy) return;
        StartCoroutine(TaskRoutine(executor));
    }

   private IEnumerator TaskRoutine(EmployeeController executor)
{
    isBusy = true;
    // Optional: Play Sound / Animation on object
    
    // Calculate total duration from action sequence
    float totalDuration = CalculateTaskDuration();
    yield return new WaitForSeconds(totalDuration);

    CompleteTask();
    isBusy = false;
}

private float CalculateTaskDuration()
{
    float total = 0f;
    
    if (data.actionSequence != null && data.actionSequence.Count > 0)
    {
        foreach (EmployeeAction action in data.actionSequence)
        {
            switch (action.actionType)
            {
                case ActionType.PlayAnimation:
                    total += action.animationDuration;
                    break;
                case ActionType.Wait:
                    total += action.waitDuration;
                    break;
                case ActionType.WalkTo:
                case ActionType.RunTo:
                    total += 2f; // Estimate walk time
                    break;
                case ActionType.Sit:
                case ActionType.StandUp:
                    total += 1.5f; // Estimate sit/stand time
                    break;
                case ActionType.LookAt:
                    total += 0.5f;
                    break;
            }
        }
    }
    else
    {
        total = 3f; // Default if no actions
    }
    
    return total;
}


    private void CompleteTask()
    {
        if (data.isDarkTask)
        {
            // Dark tasks feed the spirit
            SpiritManager.Instance.AddPower(data.spiritPowerGain);
            GameEvents.OnDarkTaskStarted?.Invoke(data); // Notify listeners
        }
        else
        {
            // Normal tasks might generate money or lower heat?
            GameEvents.OnTaskCompleted?.Invoke(data);
        }
    }
}
