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

    [Header("Collision")]
    public LayerMask collisionLayer;   // Layers the camera should collide with (e.g., Default, Ground)
    public float collisionRadius = 0.2f; // Radius for spherecast (prevents clipping through corners)
    public float minDistance = 0.5f;   // Minimum distance from player to prevent camera clipping inside player head

    private float _yaw;
    private float _pitch;

    void OnEnable()
    {
        if (lookAction != null && lookAction.action != null) 
            lookAction.action.Enable();
        
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

        // 1. Handle Rotation
        if (lookAction != null)
        {
            Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();
            _yaw += mouseDelta.x * sensitivity;
            _pitch -= mouseDelta.y * sensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        
        // 2. Calculate Desired Position (Standard Offset)
        // We calculate where the camera WANTs to be based on the offset
        Vector3 focusPoint = target.position + Vector3.up * offset.y; // Look at player's head/center
        Vector3 direction = targetRotation * Vector3.forward;
        float targetDist = Mathf.Abs(offset.z); // Assumes offset.z is negative, so we take absolute value

        Vector3 desiredPosition = target.position + targetRotation * offset;

        // 3. Handle Wall Collision
        // We cast from the focus point (player) backwards towards the camera
        // If we hit something, we clamp the distance to the hit point
        if (Physics.SphereCast(focusPoint, collisionRadius, -direction, out RaycastHit hit, targetDist, collisionLayer))
        {
            // If we hit a wall, place camera at hit distance (minus a small buffer)
            float hitDistance = hit.distance;
            // Ensure camera doesn't get too close (inside player's head)
            hitDistance = Mathf.Max(hitDistance, minDistance);
            
            desiredPosition = focusPoint - direction * hitDistance;
        }

        // 4. Apply Transforms
        transform.rotation = targetRotation;
        
        // Use Lerp for smooth movement, but snap if we hit a wall to prevent clipping flickering
        // Or you can simply set it directly for instant response: transform.position = desiredPosition;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
