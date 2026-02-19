using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq; // Needed for sorting

public class DialogueController : MonoBehaviour
{
    [System.Serializable]
    public class EventDialogue
    {
        [Tooltip("The event that must be TRUE for this dialogue to trigger")]
        public string requiredEvent; 
        
        [TextArea(3, 10)]
        public string[] dialogueLines;
        
        [Tooltip("If true, this dialogue will trigger even if the event hasn't happened yet (default fallback)")]
        public bool isDefaultDialogue = false;
    }

    [Header("Data Source")]
    public GameEventSO validEvents; // Reference to the list of events for dropdowns

    [Header("Dialogue Configuration")]
    public string characterName = "NPC";
    public float typingSpeed = 0.05f;
    
    [Tooltip("List of dialogues and their required events. The system picks the LAST matching event in the list.")]
    public List<EventDialogue> dialogues = new List<EventDialogue>();

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueIndicator;

    [Header("Input")]
    public InputActionReference interactAction;

    private bool playerInZone = false;
    private bool dialogueActive = false;
    private bool isTyping = false;
    private int currentLineIndex = 0;
    private string[] currentDialogueLines; // The specific lines currently being shown

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
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
            if (!dialogueActive)
            {
                StartDialogue();
            }
            else
            {
                if (isTyping)
                {
                    StopAllCoroutines();
                    CompleteLineInstantly();
                }
                else
                {
                    ShowNextLine();
                }
            }
        }
    }

    void StartDialogue()
    {
        dialogueActive = true;
        currentLineIndex = 0;

        if (UIManager.Instance != null) UIManager.Instance.RegisterUIOpen();

        // --- CORE LOGIC CHANGE: Select Dialogue based on Events ---
        currentDialogueLines = GetBestDialogue();

        if (currentDialogueLines == null || currentDialogueLines.Length == 0)
        {
            Debug.LogWarning("No valid dialogue found for current state.");
            EndDialogue();
            return;
        }

        // Setup UI
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (nameText != null) nameText.text = characterName;
        if (InteractionUI.Instance != null) InteractionUI.Instance.HidePrompt();

        ShowNextLine();
    }

    // Logic to find the most relevant dialogue
    string[] GetBestDialogue()
    {
        // 1. Iterate through the list backwards to find the most recent/advanced event that is TRUE
        for (int i = dialogues.Count - 1; i >= 0; i--)
        {
            var d = dialogues[i];
            
            // If it's a default dialogue, it's a candidate (lowest priority usually if at top of list)
            if (d.isDefaultDialogue) return d.dialogueLines;

            // Check if the event is set in GameEventManager
            if (GameEventManager.Instance != null && !string.IsNullOrEmpty(d.requiredEvent))
            {
                if (GameEventManager.Instance.GetFlag(d.requiredEvent))
                {
                    return d.dialogueLines;
                }
            }
        }
        
        return null; // Should ideally have a default setup
    }

    void ShowNextLine()
    {
        if (currentLineIndex < currentDialogueLines.Length)
        {
            if (continueIndicator != null) continueIndicator.SetActive(false);
            StartCoroutine(TypeLine(currentDialogueLines[currentLineIndex]));
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        if (continueIndicator != null) continueIndicator.SetActive(true);
    }

    void CompleteLineInstantly()
    {
        isTyping = false;
        if (currentLineIndex > 0 && currentLineIndex <= currentDialogueLines.Length)
        {
            dialogueText.text = currentDialogueLines[currentLineIndex - 1];
        }
        if (continueIndicator != null) continueIndicator.SetActive(true);
    }

    void EndDialogue()
    {
        dialogueActive = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (UIManager.Instance != null) UIManager.Instance.RegisterUIClose();
        
        // Optional: Trigger any "OnDialogueEnd" events here if needed later
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            if (!dialogueActive && InteractionUI.Instance != null)
                InteractionUI.Instance.ShowPrompt("Press E to Talk");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (dialogueActive)
            {
                StopAllCoroutines();
                if (dialoguePanel != null) dialoguePanel.SetActive(false);
                dialogueActive = false;
                if (UIManager.Instance != null) UIManager.Instance.RegisterUIClose();
            }
            if (InteractionUI.Instance != null) InteractionUI.Instance.HidePrompt();
        }
    }
}
