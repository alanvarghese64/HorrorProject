using UnityEngine;

public class EmailPanelController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject emailListPanel;
    public GameObject contentAreaPanel;
    
    [Header("Email Detail Panels")]
    public GameObject email1DetailPanel;
    public GameObject email2DetailPanel;
    public GameObject email3DetailPanel;
    // Add more as needed
    
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
        
        if (defaultMessagePanel != null) defaultMessagePanel.SetActive(false);
    }

    public void ShowEmail1()
    {
        Debug.Log("Showing Email 1");
        HideAllDetailPanels();
        if (email1DetailPanel != null) email1DetailPanel.SetActive(true);
    }

    public void ShowEmail2()
    {
        Debug.Log("Showing Email 2");
        HideAllDetailPanels();
        if (email2DetailPanel != null) email2DetailPanel.SetActive(true);
    }

    public void ShowEmail3()
    {
        Debug.Log("Showing Email 3");
        HideAllDetailPanels();
        if (email3DetailPanel != null) email3DetailPanel.SetActive(true);
    }

    // NEW: Called by DialogueController to unlock emails
    public void UnlockNewEmail(GameObject emailButton)
    {
        if (emailButton != null)
        {
            emailButton.SetActive(true);
            Debug.Log($"Email unlocked: {emailButton.name}");
        }
    }
}
