using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Required for New Input System

public class ZoneMessage : MonoBehaviour
{
    [Header("UI Interaction")]
    public string customMessage = ""; 

    [Header("Door Settings")]
    public bool isDoor = false;
    public Transform doorPivot; // The object that actually rotates (can be this object)
    public Vector3 openAngle = new Vector3(0, 90, 0); // Rotation offset (e.g., 90 on Y)
    public float duration = 1.0f; // How long the animation takes
    
    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private bool playerInZone = false;

    // Use a reference to your Input Action (set this in Inspector)
    [Header("Input")]
    public InputActionReference interactAction; 

    void Start()
    {
        if (doorPivot == null) doorPivot = transform; // Default to self if not assigned
        closedRotation = doorPivot.localRotation;
    }

    void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable();
    }

    void OnDisable()
    {
        if (interactAction != null) interactAction.action.Disable();
    }

    void Update()
    {
        // Only allow interaction if player is in zone AND we are a door
        if (playerInZone && isDoor && !isAnimating)
        {
            // Check if E key (or assigned action) was pressed this frame
            if (interactAction != null && interactAction.action.WasPressedThisFrame())
            {
                ToggleDoor();
            }
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        
        // Calculate where we want to go
        if (isOpen)
        {
            // Add the openAngle to the closed state
            targetRotation = closedRotation * Quaternion.Euler(openAngle);
            if (InteractionUI.Instance != null) InteractionUI.Instance.ShowPrompt("Press E to Close");
        }
        else
        {
            targetRotation = closedRotation;
            if (InteractionUI.Instance != null) InteractionUI.Instance.ShowPrompt("Press E to Open");
        }

        StartCoroutine(AnimateDoor(targetRotation));
    }

    IEnumerator AnimateDoor(Quaternion target)
    {
        isAnimating = true;
        Quaternion startRot = doorPivot.localRotation;
        float time = 0;

        while (time < duration)
        {
            doorPivot.localRotation = Quaternion.Slerp(startRot, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        doorPivot.localRotation = target;
        isAnimating = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            
            // Show UI
            if (InteractionUI.Instance != null)
            {
                string msg = GetMessage();
                InteractionUI.Instance.ShowPrompt(msg);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            
            // Hide UI
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.HidePrompt();
            }
        }
    }

    string GetMessage()
    {
        if (!string.IsNullOrEmpty(customMessage)) return customMessage;
        
        if (isDoor)
        {
            return isOpen ? "Press E to Close" : "Press E to Open";
        }
        
        // Fallback for non-door items
        return "Press E to Interact";
    }
}
