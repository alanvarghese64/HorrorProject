using UnityEngine;

public enum InteractableType { Equipment, Evidence, Task, PC, Employee }

public class Interactable : MonoBehaviour
{
    [Header("Type")]
    public InteractableType type;
    
    [Header("Equipment Settings")]
    public string equipmentName;
    public EvidenceType detectsEvidenceType;
    public GameObject equipmentPrefab;
    
    [Header("Evidence Settings")]
    public EvidenceData evidenceData;
    public bool isRevealed = false;
    
    [Header("Task Settings")]
    public TaskData taskData;
    
    // Can be extended for other types
}
