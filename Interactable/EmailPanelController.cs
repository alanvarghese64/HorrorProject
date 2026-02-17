using UnityEngine;

public class EmailPanelController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject emailListPanel;
    public GameObject contentAreaPanel;
    public Transform inboxContentParent; // The ScrollView → Content transform
    
    [Header("Email Detail Panels")]
    public GameObject email1DetailPanel;
    public GameObject email2DetailPanel;
    public GameObject email3DetailPanel;
    public GameObject email4DetailPanel;
    
    [Header("Default State")]
    public GameObject defaultMessagePanel;

    void Start()
    {
        if (emailListPanel != null) emailListPanel.SetActive(true);
        if (contentAreaPanel != null) contentAreaPanel.SetActive(true);
        
        ShowDefaultMessage();
    }

    void ShowDefaultMessage()
    {
        HideAllDetailPanels();
        
        if (defaultMessagePanel != null) 
            defaultMessagePanel.SetActive(true);
    }

    void HideAllDetailPanels()
    {
        if (email1DetailPanel != null) email1DetailPanel.SetActive(false);
        if (email2DetailPanel != null) email2DetailPanel.SetActive(false);
        if (email3DetailPanel != null) email3DetailPanel.SetActive(false);
        if (email4DetailPanel != null) email4DetailPanel.SetActive(false);
        
        if (defaultMessagePanel != null) defaultMessagePanel.SetActive(false);
    }

    public void ShowEmail1()
    {
        ShowEmail(email1DetailPanel, "Email1");
    }

    public void ShowEmail2()
    {
        ShowEmail(email2DetailPanel, "Email2");
    }

    public void ShowEmail3()
    {
        ShowEmail(email3DetailPanel, "Email3");
    }

    public void ShowEmail4()
    {
        ShowEmail(email4DetailPanel, "NPC1_EmailRead");
    }

    void ShowEmail(GameObject emailPanel, string readFlagName)
    {
        Debug.Log($"Showing email: {emailPanel?.name}");
        HideAllDetailPanels();
        
        if (emailPanel != null) 
        {
            emailPanel.SetActive(true);
            
            if (GameEventManager.Instance != null)
            {
                GameEventManager.Instance.SetFlag(readFlagName, true);
                Debug.Log($"Email marked as read: {readFlagName}");
            }
        }
    }

    // Called by DialogueController to unlock emails
    public void UnlockNewEmail(GameObject emailButton)
    {
        if (emailButton != null)
        {
            emailButton.SetActive(true);
            Debug.Log($"Email unlocked: {emailButton.name}");
            
            // Move to top of inbox
            MoveEmailToTop(emailButton);
        }
    }

    // NEW: Move email button to top of list
    void MoveEmailToTop(GameObject emailButton)
    {
        if (inboxContentParent == null) return;
        
        // Set as first sibling (top of list)
        emailButton.transform.SetAsFirstSibling();
        
        Debug.Log($"{emailButton.name} moved to top of inbox");
    }
}
