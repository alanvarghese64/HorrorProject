using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Transform cameraTransform;
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public InputActionReference interactAction;

    private void OnEnable() => interactAction.action.Enable();
    private void OnDisable() => interactAction.action.Disable();

    private void Update()
    {
        if (interactAction.action.WasPressedThisFrame())
        {
            PerformInteraction();
        }
    }

    private void PerformInteraction()
    {
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            // Try Task
            TaskInstance task = hit.collider.GetComponent<TaskInstance>();
            if (task != null)
            {
                task.Execute(null); // Player executing
                return;
            }

            // Try Employee
            EmployeeController employee = hit.collider.GetComponent<EmployeeController>();
            if (employee != null)
            {
                // In future: Open persuasion UI
                Debug.Log($"Interacting with {employee.data.employeeName}");
            }
        }
    }
}
