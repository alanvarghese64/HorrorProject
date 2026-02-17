using UnityEngine;
using UnityEngine.InputSystem;

public class LaptopInteraction : MonoBehaviour
{
    [Header("Code Settings")]
    public string correctCode = "1234";
    public int codeLength = 4;

    [Header("Camera Zoom")]
    public Transform laptopScreenPosition; // Position camera here for screen view
    public float zoomDuration = 0.5f;
    public float zoomFieldOfView = 30f; // Narrower FOV = more zoomed in (default is 60)

    [Header("UI Panels")]
    public GameObject codePanel; // Login screen with buttons
    public GameObject emailPanel; // Email inbox (shown after correct code)

    [Header("What Happens When Unlocked")]
    public DoorInteraction doorToUnlock;
    public GameObject objectToActivate;

    [Header("Input")]
    public InputActionReference interactAction;

    private bool playerInZone = false;
    private bool isZoomedIn = false;
    private bool isUnlocked = false;
    private string currentInput = "";

    private Transform originalCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private float originalFieldOfView;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        originalFieldOfView = mainCamera.fieldOfView;
        
        if (codePanel != null) codePanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(false);
    }

    void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable();
    }

    void Update()
    {
        if (!playerInZone) return;

        if (interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            if (!isZoomedIn)
            {
                ZoomIn();
            }
            else
            {
                ZoomOut();
            }
        }
    }

    void ZoomIn()
    {
        isZoomedIn = true;

        // Save original camera state
        originalCameraParent = mainCamera.transform.parent;
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;
        originalFieldOfView = mainCamera.fieldOfView;

        // Disable player movement
        DisablePlayerMovement();

        // Move camera to laptop screen
        if (laptopScreenPosition != null)
        {
            mainCamera.transform.SetParent(null); // Detach from player
            StartCoroutine(SmoothZoom(
                laptopScreenPosition.position, 
                laptopScreenPosition.rotation,
                zoomFieldOfView
            ));
        }

        // Show appropriate panel
        if (!isUnlocked)
        {
            if (codePanel != null) codePanel.SetActive(true);
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.ShowPrompt("Press E to Exit");
        }
        else
        {
            if (emailPanel != null) emailPanel.SetActive(true);
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.ShowPrompt("Press E to Exit");
        }

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ZoomOut()
    {
        isZoomedIn = false;

        // Restore camera
        mainCamera.transform.SetParent(originalCameraParent);
        StartCoroutine(SmoothZoom(
            originalCameraPosition, 
            originalCameraRotation,
            originalFieldOfView
        ));

        // Hide all panels
        if (codePanel != null) codePanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(false);

        // Enable player movement
        EnablePlayerMovement();

        // Reset input (only if not unlocked)
        if (!isUnlocked)
        {
            currentInput = "";
            UpdateCodeDisplay();
        }

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Update prompt
        if (InteractionUI.Instance != null)
            InteractionUI.Instance.ShowPrompt("Press E to Use Laptop");
    }

    System.Collections.IEnumerator SmoothZoom(Vector3 targetPos, Quaternion targetRot, float targetFOV)
    {
        float elapsed = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float startFOV = mainCamera.fieldOfView;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / zoomDuration); // Smooth ease in/out

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
        mainCamera.fieldOfView = targetFOV;
    }

    // Called by UI number buttons
    public void OnNumberButtonPressed(string digit)
    {
        if (currentInput.Length < codeLength)
        {
            currentInput += digit;
            Debug.Log("Current Code: " + currentInput);
            UpdateCodeDisplay();

            // Auto-submit when full
            if (currentInput.Length == codeLength)
            {
                Invoke(nameof(SubmitCode), 0.5f);
            }
        }
    }

    public void OnClearButtonPressed()
    {
        currentInput = "";
        UpdateCodeDisplay();
        Debug.Log("Code cleared");
    }

    void SubmitCode()
    {
        if (currentInput == correctCode)
        {
            Debug.Log("CODE CORRECT!");
            isUnlocked = true;
            ShowMessage("ACCESS GRANTED");

            // Trigger external events (door unlock, etc.)
            if (doorToUnlock != null)
            {
                doorToUnlock.UnlockDoor();
            }

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            // Transition to email panel after 1 second
            Invoke(nameof(ShowEmailPanel), 1f);
        }
        else
        {
            Debug.Log($"CODE WRONG! Entered: {currentInput}");
            ShowMessage("ACCESS DENIED");
            currentInput = "";
            UpdateCodeDisplay();
        }
    }

    void ShowEmailPanel()
    {
        // Hide code panel, show email panel
        if (codePanel != null) codePanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(true);
    }

    void UpdateCodeDisplay()
    {
        var displayText = codePanel?.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (displayText != null)
        {
            string display = currentInput;
            for (int i = currentInput.Length; i < codeLength; i++)
            {
                display += "_";
            }
            displayText.text = display;
        }
    }

    void ShowMessage(string message)
    {
        var displayText = codePanel?.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (displayText != null)
        {
            displayText.text = message;
        }
        Debug.Log(message);
    }

    void DisablePlayerMovement()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    
    if (player != null)
    {
        // Disable CharacterController
        var characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Disable PlayerMovement script
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // If you have camera rotation script, disable it too
        // var cameraLook = player.GetComponentInChildren<MouseLook>();
        // if (cameraLook != null) cameraLook.enabled = false;
    }
}

void EnablePlayerMovement()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    
    if (player != null)
    {
        // Re-enable CharacterController
        var characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // Re-enable PlayerMovement script
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Re-enable camera rotation if disabled
        // var cameraLook = player.GetComponentInChildren<MouseLook>();
        // if (cameraLook != null) cameraLook.enabled = true;
    }
}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;

            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.ShowPrompt("Press E to Use Laptop");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;

            // Force zoom out if player walks away
            if (isZoomedIn)
            {
                ZoomOut();
            }

            if (InteractionUI.Instance != null)
                InteractionUI.Instance.HidePrompt();
        }
    }
}
