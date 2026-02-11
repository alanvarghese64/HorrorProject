using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Transform cameraTransform;
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public InputActionReference interactAction;

    // Cleaned up: Removed Equipment and Evidence references that were causing errors.
    // Logic can be re-added here later.

    private void OnEnable()
    {
        if (interactAction != null && interactAction.action != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
            interactAction.action.Disable();
    }

    private void Update()
    {
        if (interactAction != null && interactAction.action != null && interactAction.action.WasPressedThisFrame())
        {
            PerformInteraction();
        }
    }

    private void PerformInteraction()
    {
        // Simple debug raycast to test interaction range
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            Debug.Log($"Raycast hit: {hit.collider.name} (Interaction logic pending)");
        }
    }
}
