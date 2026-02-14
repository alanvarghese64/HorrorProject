using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionPrompt = "Press E to interact";

    public virtual string GetPrompt()
    {
        return interactionPrompt;
    }
}
