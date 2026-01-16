using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // The Player

    [Header("Input")]
    public InputActionReference lookAction; // Assign the 'Look' action (Mouse Delta) here

    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 1.5f, -4f); // Adjust Z for distance, Y for height
    public float sensitivity = 0.2f;   // Mouse sensitivity
    public float followSpeed = 10f;    // How fast the camera catches up to position
    
    [Header("Limits")]
    public float minPitch = -30f;      // Look down limit
    public float maxPitch = 60f;       // Look up limit

    private float _yaw;
    private float _pitch;

    void OnEnable()
    {
        if (lookAction != null && lookAction.action != null) 
            lookAction.action.Enable();
        
        // Optional: Lock cursor for shooter feel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        if (lookAction != null && lookAction.action != null) 
            lookAction.action.Disable();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (!target) return;

        // 1. Handle Rotation (Controlled by Input)
        if (lookAction != null)
        {
            Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();
            
            _yaw += mouseDelta.x * sensitivity;
            _pitch -= mouseDelta.y * sensitivity; // Subtract to invert Y axis naturally
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        
        // Smoothly rotate camera (or set directly for snappy shooter feel)
        transform.rotation = targetRotation; 

        // 2. Handle Position (Follows Target + Offset)
        // We rotate the offset by the CAMERA'S rotation, not the player's
        Vector3 desiredPosition = target.position + targetRotation * offset;

        // Smooth position follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
