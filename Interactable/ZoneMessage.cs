using UnityEngine;
using TMPro; // Need this for TextMeshPro

public class ZoneMessage : MonoBehaviour
{
    public GameObject uiTextObject; // Drag the UI Text here

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiTextObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiTextObject.SetActive(false);
        }
    }
}
