using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    private EmployeeController currentEmployee;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    public void OpenConversation(EmployeeController employee, List<EvidenceData> playerEvidence)
    {
        currentEmployee = employee;
        
        // Show UI (you'll create Canvas UI for this)
        Debug.Log($"Opening conversation with {employee.data.employeeName}");
        Debug.Log($"Current Conviction: {employee.data.currentConviction}/{employee.data.convictionThreshold}");
        
        // Here you'd show your UI panel with evidence buttons
    }
    
    public void PresentEvidence(EvidenceData evidence)
    {
        EmployeeData empData = currentEmployee.data;
        
        float effectiveness = evidence.convictionPower;
        
        // Bonus if evidence matches employee preference
        if (empData.preferredEvidenceTypes.Contains(evidence.type))
        {
            effectiveness *= 2f;
            Debug.Log($"{empData.employeeName}: This is compelling evidence!");
        }
        else
        {
            effectiveness *= 0.5f;
            Debug.Log($"{empData.employeeName}: I'm not convinced by this...");
        }
        
        empData.currentConviction += effectiveness;
        
        if (empData.currentConviction >= empData.convictionThreshold)
        {
            EmployeeConvinced();
        }
    }
    
    private void EmployeeConvinced()
    {
        Debug.Log($"{currentEmployee.data.employeeName} is now convinced!");
        currentEmployee.isConvinced = true;
        GameEvents.OnEmployeeConvinced?.Invoke(currentEmployee);
        
        // Close dialogue UI
    }
}
