using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionPrompt = "Press E to interact";
    
    [Header("Input Action")]
    public InputActionReference interactAction;

    [Header("Interaction Type")]
    public InteractionType interactionType = InteractionType.PickUp;

    [Header("Animation (if applicable)")]
    public Animator targetAnimator; // For player (sit/stand) or door
    public string animationTriggerName = ""; // Primary trigger
    public string secondaryTriggerName = ""; // For Stand Up or Close Door

    [Header("Toggle State (Sit/Stand or Door Open/Close)")]
    public string primaryPrompt = "Press E to interact";
    public string secondaryPrompt = "Press E to interact";
    private bool isToggled = false; // false = standing/closed, true = sitting/open

    [Header("Player Positioning (for Sit)")]
    public Transform sitPosition; // Position & rotation for sitting
    public bool lockPlayerMovement = true;
    
    [Header("Movement Control (New Input System)")]
    public string gameplayActionMapName = "Player"; // Your main action map name
    public string seatedActionMapName = "UI"; // Action map without movement (or create "Seated")
    public float enableMovementDelay = 0.5f; // Delay after standing before movement re-enables
    
    [Header("Pickup Settings")]
    public bool destroyOnPickup = true;
    public float pickupDelay = 0f; // Delay before destroying (for animation)

    [Header("Door Settings")]
    public bool useDoorAnimator = true; // true = Animator, false = manual rotation
    public float doorRotationAngle = 90f; // For manual rotation
    public float doorSpeed = 2f; // Rotation speed
    public Transform doorHinge; // The pivot point for rotation

    private Transform playerTransform;
    private CharacterController playerController;
    private PlayerInput playerInput; // NEW: For action map switching
    private bool playerInRange = false;
    private bool isDoorAnimating = false;
    private Quaternion doorClosedRotation;
    private Quaternion doorOpenRotation;
    private Coroutine enableMovementCoroutine;

    public enum InteractionType
    {
        PickUp,
        SitDown,
        StandUp,
        ToggleSit, // Sit <-> Stand
        OpenDoor,
        CloseDoor,
        ToggleDoor // Open <-> Close
    }

    void Start()
    {
        // Cache door rotations for manual rotation
        if (doorHinge != null && !useDoorAnimator)
        {
            doorClosedRotation = doorHinge.localRotation;
            doorOpenRotation = doorClosedRotation * Quaternion.Euler(0, doorRotationAngle, 0);
        }
    }

    void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.Disable();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        playerTransform = other.transform;
        
        // Cache player components
        if (lockPlayerMovement && (interactionType == InteractionType.ToggleSit || interactionType == InteractionType.SitDown))
        {
            playerController = other.GetComponent<CharacterController>();
            playerInput = other.GetComponent<PlayerInput>(); // NEW: Cache PlayerInput
        }

        // Show appropriate prompt
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if (playerInteraction != null)
        {
            string prompt = GetCurrentPrompt();
            playerInteraction.ShowPromptFromTrigger(prompt);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;

        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if (playerInteraction != null)
        {
            playerInteraction.HidePromptFromTrigger();
        }
    }

    void Update()
    {
        if (!playerInRange) return;

        if (interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            Interact();
        }

        // Handle manual door rotation
        if (isDoorAnimating && doorHinge != null && !useDoorAnimator)
        {
            RotateDoorManually();
        }
    }

    string GetCurrentPrompt()
    {
        if (interactionType == InteractionType.ToggleSit || interactionType == InteractionType.ToggleDoor)
        {
            return isToggled ? secondaryPrompt : primaryPrompt;
        }
        return interactionPrompt;
    }

    public virtual string GetPrompt()
    {
        return GetCurrentPrompt();
    }

    public virtual void Interact()
    {
        switch (interactionType)
        {
            case InteractionType.PickUp:
                HandlePickUp();
                break;

            case InteractionType.SitDown:
                HandleSitDown();
                break;

            case InteractionType.StandUp:
                HandleStandUp();
                break;

            case InteractionType.ToggleSit:
                HandleToggleSit();
                break;

            case InteractionType.OpenDoor:
                HandleOpenDoor();
                break;

            case InteractionType.CloseDoor:
                HandleCloseDoor();
                break;

            case InteractionType.ToggleDoor:
                HandleToggleDoor();
                break;
        }
    }

    // ==================== PICKUP ====================
    void HandlePickUp()
    {
        Debug.Log($"Picked up: {gameObject.name}");
        
        // Trigger pickup animation on player (optional)
        if (targetAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            targetAnimator.SetTrigger(animationTriggerName);
        }

        // Destroy or deactivate item
        if (destroyOnPickup)
        {
            if (pickupDelay > 0)
            {
                Destroy(gameObject, pickupDelay);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // ==================== SIT DOWN ====================
    void HandleSitDown()
    {
        if (targetAnimator == null || string.IsNullOrEmpty(animationTriggerName))
        {
            Debug.LogWarning("Animator or trigger name not set!");
            return;
        }

        PositionPlayerForSitting();
        targetAnimator.SetTrigger(animationTriggerName);
        DisablePlayerMovement();
        Debug.Log("Triggered sit down animation");
    }

    // ==================== STAND UP ====================
    void HandleStandUp()
    {
        if (targetAnimator == null || string.IsNullOrEmpty(animationTriggerName))
        {
            Debug.LogWarning("Animator or trigger name not set!");
            return;
        }

        targetAnimator.SetTrigger(animationTriggerName);
        EnablePlayerMovement();
        Debug.Log("Triggered stand up animation");
    }

    // ==================== TOGGLE SIT/STAND ====================
    void HandleToggleSit()
    {
        if (targetAnimator == null)
        {
            Debug.LogWarning("Animator not assigned!");
            return;
        }

        if (!isToggled)
        {
            // Sit Down
            if (!string.IsNullOrEmpty(animationTriggerName))
            {
                PositionPlayerForSitting();
                targetAnimator.SetTrigger(animationTriggerName);
                DisablePlayerMovement();
                UpdatePromptText(secondaryPrompt);
                Debug.Log("Toggled to Sit");
            }
        }
        else
        {
            // Stand Up
            if (!string.IsNullOrEmpty(secondaryTriggerName))
            {
                targetAnimator.SetTrigger(secondaryTriggerName);
                EnablePlayerMovement();
                UpdatePromptText(primaryPrompt);
                Debug.Log("Toggled to Stand");
            }
        }

        isToggled = !isToggled;
    }

    // ==================== OPEN DOOR ====================
    void HandleOpenDoor()
    {
        if (useDoorAnimator)
        {
            if (targetAnimator == null || string.IsNullOrEmpty(animationTriggerName))
            {
                Debug.LogWarning("Door Animator or trigger not set!");
                return;
            }
            targetAnimator.SetTrigger(animationTriggerName);
        }
        else
        {
            isDoorAnimating = true;
            isToggled = true; // Mark as opening
        }
        
        Debug.Log("Opening door");
    }

    // ==================== CLOSE DOOR ====================
    void HandleCloseDoor()
    {
        if (useDoorAnimator)
        {
            if (targetAnimator == null || string.IsNullOrEmpty(secondaryTriggerName))
            {
                Debug.LogWarning("Door Animator or close trigger not set!");
                return;
            }
            targetAnimator.SetTrigger(secondaryTriggerName);
        }
        else
        {
            isDoorAnimating = true;
            isToggled = false; // Mark as closing
        }
        
        Debug.Log("Closing door");
    }

    // ==================== TOGGLE DOOR ====================
    void HandleToggleDoor()
    {
        if (useDoorAnimator)
        {
            if (targetAnimator == null)
            {
                Debug.LogWarning("Door Animator not assigned!");
                return;
            }

            if (!isToggled)
            {
                // Open
                if (!string.IsNullOrEmpty(animationTriggerName))
                {
                    targetAnimator.SetTrigger(animationTriggerName);
                    UpdatePromptText(secondaryPrompt);
                    Debug.Log("Door opened");
                }
            }
            else
            {
                // Close
                if (!string.IsNullOrEmpty(secondaryTriggerName))
                {
                    targetAnimator.SetTrigger(secondaryTriggerName);
                    UpdatePromptText(primaryPrompt);
                    Debug.Log("Door closed");
                }
            }
        }
        else
        {
            // Manual rotation toggle
            isDoorAnimating = true;
            UpdatePromptText(isToggled ? primaryPrompt : secondaryPrompt);
        }

        isToggled = !isToggled;
    }

    // ==================== MANUAL DOOR ROTATION ====================
    void RotateDoorManually()
    {
        Quaternion targetRotation = isToggled ? doorOpenRotation : doorClosedRotation;
        doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, targetRotation, Time.deltaTime * doorSpeed);

        if (Quaternion.Angle(doorHinge.localRotation, targetRotation) < 0.5f)
        {
            doorHinge.localRotation = targetRotation;
            isDoorAnimating = false;
        }
    }

    // ==================== PLAYER POSITIONING ====================
    void PositionPlayerForSitting()
    {
        if (sitPosition == null || playerTransform == null) return;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        playerTransform.position = sitPosition.position;
        playerTransform.rotation = sitPosition.rotation;

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    // ==================== MOVEMENT CONTROL (NEW INPUT SYSTEM) ====================
    void DisablePlayerMovement()
    {
        if (!lockPlayerMovement) return;

        if (playerInput != null)
        {
            // Switch to action map without movement controls
            playerInput.SwitchCurrentActionMap(seatedActionMapName);
            Debug.Log($"Switched to action map: {seatedActionMapName} - Movement DISABLED");
        }
        else
        {
            Debug.LogWarning("PlayerInput component not found! Movement won't be locked.");
        }
    }

    void EnablePlayerMovement()
    {
        if (!lockPlayerMovement) return;

        if (playerInput != null)
        {
            // Stop any existing coroutine
            if (enableMovementCoroutine != null)
            {
                StopCoroutine(enableMovementCoroutine);
            }

            // Delay re-enabling movement to let stand animation play
            enableMovementCoroutine = StartCoroutine(EnableMovementDelayed());
        }
    }

    IEnumerator EnableMovementDelayed()
    {
        yield return new WaitForSeconds(enableMovementDelay);

        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(gameplayActionMapName);
            Debug.Log($"Switched to action map: {gameplayActionMapName} - Movement ENABLED");
        }
    }

    void UpdatePromptText(string newPrompt)
    {
        if (!playerInRange || playerTransform == null) return;
        
        PlayerInteraction playerInteraction = playerTransform.GetComponent<PlayerInteraction>();
        if (playerInteraction != null)
        {
            playerInteraction.ShowPromptFromTrigger(newPrompt);
        }
    }

    // ==================== ANIMATION EVENT (OPTIONAL) ====================
    // Call this from an Animation Event at the end of your StandUp animation for precise timing
    public void OnStandUpAnimationComplete()
    {
        if (playerInput != null && lockPlayerMovement)
        {
            playerInput.SwitchCurrentActionMap(gameplayActionMapName);
            Debug.Log($"Animation Event: Movement ENABLED via {gameplayActionMapName}");
        }
    }
}
