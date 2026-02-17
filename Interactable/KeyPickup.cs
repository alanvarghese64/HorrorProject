using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public string itemName = "Key";
    public DoorInteraction doorToUnlock; // For unlocking doors directly

    [Header("Code Digit Settings (Optional)")]
    public bool isCodeDigit = false; // NEW: Check this for code digits
    public string digitValue = "0"; // The digit (0-9)
    public int digitPosition = 0; // Position in code (0-3)

    [Header("Input")]
    public InputActionReference interactAction;

    private bool playerInZone = false;

    void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable();
    }

    void Update()
    {
        if (!playerInZone) return;

        if (interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            PickupItem();
        }
    }

    void PickupItem()
    {
        Debug.Log("Picked up " + itemName);

        // If it's a code digit, add to inventory
        if (isCodeDigit)
        {
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddCodeDigit(digitPosition, digitValue);
                Debug.Log($"Code Digit: Position {digitPosition} = {digitValue}");
            }
        }
        else
        {
            // Regular key - unlock door
            if (doorToUnlock != null)
            {
                doorToUnlock.UnlockDoor();
            }
        }

        if (InteractionUI.Instance != null) InteractionUI.Instance.HidePrompt();
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            
            if (InteractionUI.Instance != null)
            {
                string prompt = isCodeDigit ? $"Press E to Pick Up Digit" : $"Press E to Pick Up {itemName}";
                InteractionUI.Instance.ShowPrompt(prompt);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (InteractionUI.Instance != null) InteractionUI.Instance.HidePrompt();
        }
    }
}
