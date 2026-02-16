using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public string itemName = "Key";
    public DoorInteraction doorToUnlock; // Drag the Door GameObject here

    [Header("Input")]
    public InputActionReference interactAction;

    private bool playerInZone = false;

    void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable();
    }

    // REMOVED OnDisable - don't disable the shared action!

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

        // Unlock door remotely
        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor();
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
                InteractionUI.Instance.ShowPrompt("Press E to Pick Up " + itemName);
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
