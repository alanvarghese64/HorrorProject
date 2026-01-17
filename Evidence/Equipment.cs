using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string equipmentName; // "Thermometer", "Camera", "EMF Reader"
    public EvidenceType detectsEvidenceType; // What evidence it can find
    public GameObject equipmentPrefab; // For player's hand
}
