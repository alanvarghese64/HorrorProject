using UnityEngine;

public class ZoneMessage : MonoBehaviour
{
    [Header("Optional Override")]
    public string customMessage = ""; // Leave empty to use Tag logic

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string msg = GetMessageFromTag();
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.ShowPrompt(msg);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.HidePrompt();
            }
        }
    }

    string GetMessageFromTag()
    {
        // 1. Use custom message if set in Inspector
        if (!string.IsNullOrEmpty(customMessage)) return customMessage;

        // 2. Otherwise, check this object's Tag
        if (gameObject.CompareTag("Door"))
        {
            return "Press E to Open Door";
        }
        else if (gameObject.CompareTag("Item") || gameObject.CompareTag("Pickup"))
        {
            return "Press E to Pick Up";
        }
        else if (gameObject.CompareTag("Chair") || gameObject.CompareTag("Sit"))
        {
            return "Press E to Sit";
        }

        // 3. Default fallback
        return "Press E to Interact";
    }
}
