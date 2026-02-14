using UnityEngine;
using TMPro; // for text

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public TextMeshProUGUI interactionText;

    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    private Interactable currentInteractable;

    void Update()
    {
        CheckForInteractable();
    }

 void CheckForInteractable()
{
    if (cameraTransform == null) return;

    // TEMP: no layermask
    if (Physics.Raycast(cameraTransform.position,
                        cameraTransform.forward,
                        out RaycastHit hit,
                        interactDistance)) // ← removed interactLayer
    {
        Debug.Log($"Raycast hit: {hit.collider.gameObject.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

        Interactable interactable = hit.collider.GetComponent<Interactable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            ShowPrompt(interactable.GetPrompt());
            return;
        }
        else
        {
            Debug.Log("Hit object has NO Interactable component");
        }
    }

    currentInteractable = null;
    HidePrompt();
}


    void ShowPrompt(string text)
    {
        if (interactionText == null) return;

        interactionText.gameObject.SetActive(true);
        interactionText.text = text;
    }

    void HidePrompt()
    {
        if (interactionText == null) return;

        interactionText.gameObject.SetActive(false);
    }

    
}
