using UnityEngine;
using System.Collections.Generic;

public class PCTerminal : MonoBehaviour
{
    public List<TaskAssignment> GetAllTaskAssignments()
    {
        List<TaskAssignment> assignments = new List<TaskAssignment>();
        
        EmployeeController[] employees = FindObjectsByType<EmployeeController>(FindObjectsSortMode.None);

        
        foreach (EmployeeController emp in employees)
        {
            // Current task
            if (emp.currentTask != null)
            {
                assignments.Add(new TaskAssignment
                {
                    employeeName = emp.data.employeeName,
                    task = emp.currentTask,
                    status = "IN PROGRESS"
                });
            }
            
            // Show upcoming tasks from their sequence
            for (int i = emp.currentTaskIndex; i < emp.data.assignedTaskSequence.Count; i++)
            {
                assignments.Add(new TaskAssignment
                {
                    employeeName = emp.data.employeeName,
                    task = emp.data.assignedTaskSequence[i],
                    status = "QUEUED"
                });
            }
        }
        
        return assignments;
    }
}

[System.Serializable]
public class TaskAssignment
{
    public string employeeName;
    public TaskData task;
    public string status;
}
