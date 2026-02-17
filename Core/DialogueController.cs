using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [Header("Event Flag Settings")]
    public string dialogueEventFlag = "NPC1_DialogueSeen"; // Unique flag name for this NPC
    public string emailReadFlag = "NPC1_EmailRead"; // Flag when email is read

    [Header("Dialogue Settings")]
    [TextArea(3, 10)]
    public string[] dialogueLines;
    [TextArea(3, 10)]
    public string[] repeatDialogueLines; // Shown after email is unlocked but not read
    public string characterName = "NPC";
    public float typingSpeed = 0.05f;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueIndicator;

    [Header("What Happens After Dialogue")]
    public GameObject emailToUnlock; // The email button in inbox
    public EmailPanelController emailPanelController;

    [Header("Input")]
    public InputActionReference interactAction;

    private bool playerInZone = false;
    private bool dialogueActive = false;
    private bool isTyping = false;
    private int currentLineIndex = 0;
    private string[] currentDialogueSet; // Which dialogue to show

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Initially hide the email button
        if (emailToUnlock != null)
            emailToUnlock.SetActive(false);
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
            else if (dialogueActive)
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

        // Check which dialogue to show
        bool dialogueSeen = GameEventManager.Instance != null && 
                            GameEventManager.Instance.GetFlag(dialogueEventFlag);
        bool emailRead = GameEventManager.Instance != null && 
                         GameEventManager.Instance.GetFlag(emailReadFlag);

        if (emailRead)
        {
            // Email was read, show a simple acknowledgment and don't repeat
            ShowCompletionMessage();
            return;
        }
        else if (dialogueSeen && repeatDialogueLines.Length > 0)
        {
            // Dialogue seen but email not read - show repeat dialogue
            currentDialogueSet = repeatDialogueLines;
            Debug.Log("Showing repeat dialogue (email not checked yet)");
        }
        else
        {
            // First time dialogue
            currentDialogueSet = dialogueLines;
            Debug.Log("Showing first-time dialogue");
        }

        // Show dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (nameText != null)
            nameText.text = characterName;

        if (InteractionUI.Instance != null)
            InteractionUI.Instance.HidePrompt();

        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLineIndex < currentDialogueSet.Length)
        {
            if (continueIndicator != null)
                continueIndicator.SetActive(false);

            StartCoroutine(TypeLine(currentDialogueSet[currentLineIndex]));
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

        if (continueIndicator != null)
            continueIndicator.SetActive(true);
    }

    void CompleteLineInstantly()
    {
        isTyping = false;
        if (currentLineIndex > 0 && currentLineIndex <= currentDialogueSet.Length)
        {
            dialogueText.text = currentDialogueSet[currentLineIndex - 1];
        }

        if (continueIndicator != null)
            continueIndicator.SetActive(true);
    }

    void EndDialogue()
    {
        dialogueActive = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Mark dialogue as seen
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.SetFlag(dialogueEventFlag, true);
        }

        // Check if this is first time - unlock email
        bool emailAlreadyUnlocked = GameEventManager.Instance != null && 
                                    GameEventManager.Instance.GetFlag(dialogueEventFlag + "_EmailUnlocked");

        if (!emailAlreadyUnlocked && emailToUnlock != null && emailPanelController != null)
        {
            emailPanelController.UnlockNewEmail(emailToUnlock);
            
            // Mark email as unlocked
            if (GameEventManager.Instance != null)
            {
                GameEventManager.Instance.SetFlag(dialogueEventFlag + "_EmailUnlocked", true);
            }

            Debug.Log("New email unlocked!");

            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.ShowPrompt("New email received!");
                Invoke(nameof(HideInteractionPrompt), 2f);
            }
        }
        else
        {
            // Email already unlocked, remind to check it
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.ShowPrompt("Check your email at the laptop.");
                Invoke(nameof(HideInteractionPrompt), 2f);
            }
        }
    }

    void ShowCompletionMessage()
    {
        // Email was read - just show a brief message
        if (InteractionUI.Instance != null)
        {
            InteractionUI.Instance.ShowPrompt("Thanks for checking the email!");
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

            if (!dialogueActive && InteractionUI.Instance != null)
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
