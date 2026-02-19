using UnityEngine;
using System.Collections.Generic;

public class EmailPanelController : MonoBehaviour
{
    [System.Serializable]
    public class EmailEntry
    {
        [Tooltip("Unique ID for this email (e.g., NPC1_Email1)")]
        public string id;
        
        [Tooltip("The button in the inbox list")]
        public GameObject button;
        
        [Tooltip("The detail panel showing email content")]
        public GameObject detailPanel;
        
        [Tooltip("Flag that must be TRUE for this email to appear in inbox")]
        public string unlockFlag;
        
        [Tooltip("Flag set to TRUE when player opens/reads this email")]
        public string readFlag;
        
        [Tooltip("Optional: Flag to set TRUE when email is opened (chains to next event)")]
        public string onOpenFlag;
    }

    [Header("UI References")]
    public GameObject emailListPanel;
    public GameObject contentAreaPanel;
    public Transform inboxContentParent; // The ScrollView → Content transform
    
    [Header("Default State")]
    public GameObject defaultMessagePanel;

    [Header("Email System - Scalable")]
    [Tooltip("List of all emails with their unlock/read/chain flags")]
    public List<EmailEntry> emails = new List<EmailEntry>();

    void Start()
    {
        if (emailListPanel != null) emailListPanel.SetActive(true);
        if (contentAreaPanel != null) contentAreaPanel.SetActive(true);
        
        ShowDefaultMessage();
        
        // Check and unlock emails based on flags
        RefreshInbox();
    }

    void OnEnable()
    {
        // Check again when the email panel is opened (in case events happened while laptop was closed)
        RefreshInbox();
    }

    // Check all emails and show/hide buttons based on unlock flags
    void RefreshInbox()
    {
        if (GameEventManager.Instance == null) return;

        foreach (var email in emails)
        {
            if (email == null || email.button == null) continue;

            // Check if this email should be unlocked
            bool shouldUnlock = !string.IsNullOrEmpty(email.unlockFlag) && 
                               GameEventManager.Instance.GetFlag(email.unlockFlag);

            // Only unlock if not already active
            if (shouldUnlock && !email.button.activeSelf)
            {
                email.button.SetActive(true);
                MoveEmailToTop(email.button);
                Debug.Log($"Email unlocked: {email.id} via flag: {email.unlockFlag}");
            }
        }
    }

    // Open a specific email by ID (called from button OnClick)
    public void OpenEmail(string emailId)
    {
        HideAllDetailPanels();

        var email = emails.Find(x => x != null && x.id == emailId);
        
        if (email == null)
        {
            Debug.LogWarning($"Email with ID '{emailId}' not found!");
            return;
        }

        if (email.detailPanel == null)
        {
            Debug.LogWarning($"Email '{emailId}' has no detail panel assigned!");
            return;
        }

        // Show the email content
        email.detailPanel.SetActive(true);
        Debug.Log($"Opened email: {emailId}");

        // Set flags when email is opened
        if (GameEventManager.Instance != null)
        {
            // Mark as read
            if (!string.IsNullOrEmpty(email.readFlag))
            {
                GameEventManager.Instance.SetFlag(email.readFlag, true);
                Debug.Log($"Email marked as read: {email.readFlag}");
            }

            // Trigger chain event (for next dialogue, etc.)
            if (!string.IsNullOrEmpty(email.onOpenFlag))
            {
                GameEventManager.Instance.SetFlag(email.onOpenFlag, true);
                Debug.Log($"Email triggered chain event: {email.onOpenFlag}");
            }
        }
    }

    void ShowDefaultMessage()
    {
        HideAllDetailPanels();
        
        if (defaultMessagePanel != null) 
            defaultMessagePanel.SetActive(true);
    }

    void HideAllDetailPanels()
    {
        // Hide all email detail panels
        foreach (var email in emails)
        {
            if (email != null && email.detailPanel != null)
                email.detailPanel.SetActive(false);
        }
        
        if (defaultMessagePanel != null) 
            defaultMessagePanel.SetActive(false);
    }

    void MoveEmailToTop(GameObject emailButton)
    {
        if (inboxContentParent == null) return;
        
        // Set as first sibling (top of list)
        emailButton.transform.SetAsFirstSibling();
        
        Debug.Log($"{emailButton.name} moved to top of inbox");
    }

    // LEGACY SUPPORT: Keep old methods so existing buttons still work
    // You can delete these once you've updated all button OnClick events to use OpenEmail(string)
    
    public void ShowEmail1()
    {
        OpenEmail("Email1");
    }

    public void ShowEmail2()
    {
        OpenEmail("Email2");
    }

    public void ShowEmail3()
    {
        OpenEmail("Email3");
    }

    public void ShowEmail4()
    {
        OpenEmail("Email4");
    }
}
