using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [TextArea(3, 10)]
    public string[] dialogueLines;
    public string characterName = "NPC"; // Name shown in dialogue box
    public float typingSpeed = 0.05f;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel; // The entire dialogue UI panel
    public TextMeshProUGUI nameText; // Character name display
    public TextMeshProUGUI dialogueText; // The dialogue text
    public GameObject continueIndicator; // Arrow/icon showing "press to continue"

    [Header("What Happens After Dialogue")]
    public GameObject emailToUnlock;
    public EmailPanelController emailPanelController;

    [Header("Input")]
    public InputActionReference interactAction;

    private bool playerInZone = false;
    private bool dialogueActive = false;
    private bool dialogueComplete = false;
    private bool isTyping = false;
    private int currentLineIndex = 0;

    void Start()
    {
        // Hide dialogue panel at start
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
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
            if (!dialogueActive && !dialogueComplete)
            {
                // Start dialogue
                StartDialogue();
            }
            else if (dialogueActive)
            {
                if (isTyping)
                {
                    // Skip typing animation
                    StopAllCoroutines();
                    CompleteLineInstantly();
                }
                else
                {
                    // Show next line
                    ShowNextLine();
                }
            }
        }
    }

    void StartDialogue()
    {
        dialogueActive = true;
        currentLineIndex = 0;

        // Show dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Set character name
        if (nameText != null)
            nameText.text = characterName;

        // Hide interaction prompt
        if (InteractionUI.Instance != null)
            InteractionUI.Instance.HidePrompt();

        // Show first line
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            // Hide continue indicator while typing
            if (continueIndicator != null)
                continueIndicator.SetActive(false);

            StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
            currentLineIndex++;
        }
        else
        {
            // Dialogue finished
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

        // Show continue indicator when line is complete
        if (continueIndicator != null)
            continueIndicator.SetActive(true);
    }

    void CompleteLineInstantly()
    {
        isTyping = false;
        if (currentLineIndex > 0 && currentLineIndex <= dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex - 1];
        }

        // Show continue indicator
        if (continueIndicator != null)
            continueIndicator.SetActive(true);
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialogueComplete = true;

        // Hide dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Unlock new email
        if (emailToUnlock != null && emailPanelController != null)
        {
            emailPanelController.UnlockNewEmail(emailToUnlock);
            Debug.Log("New email unlocked!");
        }

        // Show completion message
        if (InteractionUI.Instance != null)
        {
            InteractionUI.Instance.ShowPrompt("New email received!");
            Invoke(nameof(HideInteractionPrompt), 2f);
        }
    }

    void HideInteractionPrompt()
    {
        if (InteractionUI.Instance != null)
            InteractionUI.Instance.HidePrompt();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;

            if (!dialogueComplete && !dialogueActive && InteractionUI.Instance != null)
            {
                InteractionUI.Instance.ShowPrompt("Press E to Talk");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;

            // Force close dialogue if player walks away
            if (dialogueActive)
            {
                StopAllCoroutines();
                if (dialoguePanel != null)
                    dialoguePanel.SetActive(false);
                dialogueActive = false;
            }

            if (InteractionUI.Instance != null)
                InteractionUI.Instance.HidePrompt();
        }
    }
}
