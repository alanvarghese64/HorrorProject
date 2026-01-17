using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; // ADD THIS LINE

public class PlayerInteraction : MonoBehaviour
{
    public Transform cameraTransform;
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public InputActionReference interactAction;

    [Header("Equipment")]
public InputActionReference useEquipmentAction; // Assign in Inspector
private GameObject currentEquipment;
private EvidenceType currentEquipmentType;
private Dictionary<string, GameObject> heldEquipment = new Dictionary<string, GameObject>();

[Header("Evidence")]
public List<EvidenceData> collectedEvidence = new List<EvidenceData>();

private void OnEnable()
{
    interactAction.action.Enable();
    useEquipmentAction.action.Enable(); // NEW
}

private void OnDisable()
{
    interactAction.action.Disable();
    useEquipmentAction.action.Disable(); // NEW
}


    private void Update()
{
    if (interactAction.action.WasPressedThisFrame())
    {
        PerformInteraction();
    }
    
    // NEW: Use equipment
    if (useEquipmentAction.action.WasPressedThisFrame() && currentEquipment != null)
    {
        UseEquipment();
    }
}


  private void PerformInteraction()
{
    if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactDistance, interactLayer))
    {
        // PC Terminal
        PCTerminal pc = hit.collider.GetComponent<PCTerminal>();
        if (pc != null)
        {
            // You'll create TaskViewUI later
            Debug.Log("Opening PC - Task List");
            // TaskViewUI.Instance.Show(pc.GetAllTaskAssignments());
            return;
        }
        
        // Equipment pickup
        Equipment equipment = hit.collider.GetComponent<Equipment>();
        if (equipment != null)
        {
            PickupEquipment(equipment);
            return;
        }
        
        // Task (keep existing)
        TaskInstance task = hit.collider.GetComponent<TaskInstance>();
        if (task != null)
        {
            task.Execute(null);
            return;
        }

        // Employee conversation
        EmployeeController employee = hit.collider.GetComponent<EmployeeController>();
        if (employee != null)
        {
            DialogueManager.Instance.OpenConversation(employee, collectedEvidence);
            return;
        }
    }
}

private void PickupEquipment(Equipment equip)
{
    if (!heldEquipment.ContainsKey(equip.equipmentName))
    {
        heldEquipment.Add(equip.equipmentName, equip.equipmentPrefab);
        
        // Spawn on camera (or hand bone)
        currentEquipment = Instantiate(equip.equipmentPrefab, cameraTransform);
        currentEquipmentType = equip.detectsEvidenceType;
        
        Destroy(equip.gameObject);
        
        Debug.Log($"Picked up {equip.equipmentName}");
    }
}

private void UseEquipment()
{
    if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactDistance))
    {
        EvidenceZone zone = hit.collider.GetComponent<EvidenceZone>();
        
        if (zone != null && !zone.isRevealed)
        {
            if (zone.evidenceData.type == currentEquipmentType)
            {
                CollectEvidence(zone.evidenceData);
                zone.isRevealed = true;
            }
            else
            {
                Debug.Log("Wrong equipment for this evidence");
            }
        }
    }
}

private void CollectEvidence(EvidenceData evidence)
{
    collectedEvidence.Add(evidence);
    Debug.Log($"Evidence Collected: {evidence.evidenceName}");
    // Show UI notification
}

}
