using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LaptopInteraction : MonoBehaviour
{
    [Header("Code Settings")]
    public string correctCode = "1234";
    public int codeLength = 4;

    [Header("Camera Zoom")]
    public Transform laptopScreenPosition;
    public float zoomDuration = 0.5f;
    public float zoomFieldOfView = 30f;

    [Header("UI Panels")]
    public GameObject codePanel;
    public GameObject emailPanel;
    
    [Header("Code Panel UI Elements")]
    public TextMeshProUGUI codeDisplayText; // Main code display (large)
    public TextMeshProUGUI messageText; // Feedback messages (small)

    [Header("What Happens When Unlocked")]
    public DoorInteraction doorToUnlock;
    public GameObject objectToActivate;

    // Add this field with other [Header] sections
[Header("Event Triggers")]
public string eventOnUnlock = ""; // Event to trigger when laptop unlocked

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
        if (messageText != null) messageText.gameObject.SetActive(false);
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

        // NOTIFY UI MANAGER - NEW
    if (UIManager.Instance != null)
    {
        UIManager.Instance.RegisterUIOpen();
    }

        originalCameraParent = mainCamera.transform.parent;
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;
        originalFieldOfView = mainCamera.fieldOfView;

        DisablePlayerMovement();

        if (laptopScreenPosition != null)
        {
            mainCamera.transform.SetParent(null);
            StartCoroutine(SmoothZoom(
                laptopScreenPosition.position, 
                laptopScreenPosition.rotation,
                zoomFieldOfView
            ));
        }

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ZoomOut()
    {
        isZoomedIn = false;

        mainCamera.transform.SetParent(originalCameraParent);
        StartCoroutine(SmoothZoom(
            originalCameraPosition, 
            originalCameraRotation,
            originalFieldOfView
        ));

        if (codePanel != null) codePanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(false);
         if (UIManager.Instance != null)
    {
        UIManager.Instance.RegisterUIClose();
    }

        EnablePlayerMovement();

        if (!isUnlocked)
        {
            currentInput = "";
            UpdateCodeDisplay();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
            float t = Mathf.SmoothStep(0f, 1f, elapsed / zoomDuration);

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
        mainCamera.fieldOfView = targetFOV;
    }

    public void OnNumberButtonPressed(string digit)
    {
        if (currentInput.Length < codeLength)
        {
            currentInput += digit;
            Debug.Log("Current Code: " + currentInput);
            UpdateCodeDisplay();

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
        HideMessage();
        Debug.Log("Code cleared");
    }

   void SubmitCode()
{
    if (currentInput == correctCode)
    {
        Debug.Log("CODE CORRECT!");
        isUnlocked = true;
        ShowMessage("ACCESS GRANTED", Color.green);

        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor();
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
        
        // NEW: Trigger event
        if (!string.IsNullOrEmpty(eventOnUnlock) && GameEventManager.Instance != null)
        {
            GameEventManager.Instance.TriggerEvent(eventOnUnlock);
        }

        Invoke(nameof(ShowEmailPanel), 1.5f);
    }
    else
    {
        // ... rest of code
    }
}

    void ClearAfterError()
    {
        UpdateCodeDisplay();
        HideMessage();
    }

    void ShowEmailPanel()
    {
        if (codePanel != null) codePanel.SetActive(false);
        if (emailPanel != null) emailPanel.SetActive(true);
    }

  void UpdateCodeDisplay()
{
    if (codeDisplayText != null)
    {
        // Define colors for each position
        string[] colors = { "red", "#38C7EF", "green", "orange" };
        
        string display = "";
        
        for (int i = 0; i < codeLength; i++)
        {
            if (i < currentInput.Length)
            {
                // Show entered digit in white (or keep colored)
                display += $"<color={colors[i]}>{currentInput[i]}</color> ";
            }
            else
            {
                // Show underscore placeholder in color
                display += $"<color={colors[i]}>_</color> ";
            }
        }
        
        codeDisplayText.text = display.TrimEnd();
    }
}


    // NEW: Show small message
    void ShowMessage(string message, Color color)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            messageText.gameObject.SetActive(true);
        }
    }

    // NEW: Hide message
    void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    void DisablePlayerMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            var characterController = player.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            var playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }
    }

    void EnablePlayerMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            var characterController = player.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = true;
            }

            var playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
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

            if (isZoomedIn)
            {
                ZoomOut();
            }

            if (InteractionUI.Instance != null)
                InteractionUI.Instance.HidePrompt();
        }
    }
}
