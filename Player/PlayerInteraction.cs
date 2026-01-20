using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    public Transform cameraTransform;
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public InputActionReference interactAction;

    [Header("Equipment")]
    public InputActionReference useEquipmentAction;
    private GameObject currentEquipment;
    private EvidenceType currentEquipmentType;
    private Dictionary<string, GameObject> heldEquipment = new Dictionary<string, GameObject>();

    [Header("Evidence")]
    public List<EvidenceData> collectedEvidence = new List<EvidenceData>();

    private void OnEnable()
    {
        interactAction.action.Enable();
        useEquipmentAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.Disable();
        useEquipmentAction.action.Disable();
    }

    private void Update()
    {
        if (interactAction.action.WasPressedThisFrame())
        {
            PerformInteraction();
        }
        
        if (useEquipmentAction.action.WasPressedThisFrame() && currentEquipment != null)
        {
            UseEquipment();
        }
    }

    private void PerformInteraction()
    {
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            // Check for Interactable component
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                HandleInteractable(interactable);
                return;
            }
            
            // Check for Employee directly
            EmployeeController employee = hit.collider.GetComponent<EmployeeController>();
            if (employee != null)
            {
                DialogueManager.Instance.OpenConversation(employee, collectedEvidence);
                return;
            }
            
            // Legacy PCTerminal check
            PCTerminal pc = hit.collider.GetComponent<PCTerminal>();
            if (pc != null)
            {
                Debug.Log("Opening PC - Task List");
                return;
            }
        }
    }

    private void HandleInteractable(Interactable interactable)
    {
        switch (interactable.type)
        {
            case InteractableType.Equipment:
                PickupEquipment(interactable);
                break;
                
            case InteractableType.PC:
                OpenPC();
                break;
                
            case InteractableType.Evidence:
                // Evidence is handled by UseEquipment()
                break;
        }
    }

    private void OpenPC()
    {
        Debug.Log("Opening PC - Task List");
        // TaskViewUI.Instance.Show(GameManager.Instance.GetAllTaskAssignments());
    }

    private void PickupEquipment(Interactable interactable)
    {
        if (!heldEquipment.ContainsKey(interactable.equipmentName))
        {
            heldEquipment.Add(interactable.equipmentName, interactable.equipmentPrefab);
            currentEquipment = Instantiate(interactable.equipmentPrefab, cameraTransform);
            currentEquipmentType = interactable.detectsEvidenceType;
            Destroy(interactable.gameObject);
            Debug.Log($"Picked up {interactable.equipmentName}");
        }
    }

    private void UseEquipment()
    {
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            
            if (interactable != null && interactable.type == InteractableType.Evidence && !interactable.isRevealed)
            {
                if (interactable.evidenceData.type == currentEquipmentType)
                {
                    CollectEvidence(interactable.evidenceData);
                    interactable.isRevealed = true;
                    Debug.Log($"Evidence found: {interactable.evidenceData.evidenceName}");
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
    }
}
