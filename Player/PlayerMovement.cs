using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform cameraTransform; // Drag Main Camera here

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference sprintAction;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Fallback if camera isn't assigned
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (sprintAction != null) sprintAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (sprintAction != null) sprintAction.action.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        bool isRunning = sprintAction.action.IsPressed();

        if (input.sqrMagnitude < 0.01f)
        {
            ApplyGravity();
            return;
        }

        // --- CAMERA RELATIVE MOVEMENT ---
        // Get Camera's forward and right, ignoring vertical tilt (Y)
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Calculate move direction relative to camera
        Vector3 moveDirection = (camRight * input.x + camForward * input.y).normalized;
        // --------------------------------

        float speed = isRunning ? runSpeed : walkSpeed;

        // Move the controller
        controller.Move(moveDirection * speed * Time.deltaTime);

        // Rotate character to face movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }

        ApplyGravity();
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAnimation()
    {
        // Simple blend tree drive
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        bool isRunning = sprintAction.action.IsPressed();
        float targetSpeed = input.magnitude * (isRunning ? 1f : 0.5f);
        
        animator.SetFloat("Speed", targetSpeed, 0.1f, Time.deltaTime);
    }
}
