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
        
        yield return new WaitForSeconds(data.duration);

        CompleteTask();
        isBusy = false;
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
