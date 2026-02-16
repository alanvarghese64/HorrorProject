using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isLocked = false;
    public string lockedMessage = "Locked";
    
    public Transform doorPivot; 
    public Vector3 openAngle = new Vector3(0, 90, 0); 
    public float duration = 1.0f; 
    
    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private bool playerInZone = false;

    [Header("Input")]
    public InputActionReference interactAction; 

    void Start()
    {
        if (doorPivot == null) doorPivot = transform; 
        closedRotation = doorPivot.localRotation;
    }

    void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable();
    }

    // REMOVED OnDisable - don't disable the shared action!

    void Update()
    {
        if (!playerInZone) return;
        if (isAnimating) return;

        if (interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            TryOpenDoor();
        }
    }

    // Called remotely by KeyPickup
    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log(gameObject.name + " is now UNLOCKED");
        
        // Update prompt if player is already at the door
        if (playerInZone) RefreshPrompt();
    }

    void TryOpenDoor()
    {
        if (isLocked)
        {
            Debug.Log("Door is LOCKED");
            if (InteractionUI.Instance != null) 
                InteractionUI.Instance.ShowPrompt(lockedMessage);
            return;
        }

        Debug.Log("Opening/Closing door");
        ToggleDoor();
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        
        Quaternion target = isOpen ? closedRotation * Quaternion.Euler(openAngle) : closedRotation;
        
        string nextPrompt = isOpen ? "Press E to Close" : "Press E to Open";
        if (InteractionUI.Instance != null) InteractionUI.Instance.ShowPrompt(nextPrompt);

        StartCoroutine(AnimateDoor(target));
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
            RefreshPrompt();
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

    void RefreshPrompt()
    {
        if (InteractionUI.Instance == null) return;

        string msg;
        if (isLocked)
            msg = lockedMessage;
        else
            msg = isOpen ? "Press E to Close" : "Press E to Open";

        InteractionUI.Instance.ShowPrompt(msg);
    }
}
