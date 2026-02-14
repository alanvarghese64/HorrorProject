using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class GhostAI : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;
    public Transform playerCamera;

    [Header("Settings")]
    public float runSpeed = 5f;
    public float stoppingDistance = 1.5f;

    [Header("Dynamic Detection")]
    public float maxDetectionAngle = 60f; // Angle when far away
    public float minDetectionAngle = 20f; // Angle when very close (harder to stop)
    public float angleReductionDistance = 5f; // Distance at which angle starts shrinking

    [Header("Aggression")]
    public float forceChaseDistance = 15f; 

    [Header("Visibility Logic")]
    public LayerMask viewBlockerLayer; // Ensure Player is UNCHECKED here
    
    private float updateRate = 0.2f; 
    private float nextUpdateTime;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isVisible;
    private Renderer ghostRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ghostRenderer = GetComponentInChildren<Renderer>();

        agent.speed = runSpeed;
        agent.stoppingDistance = stoppingDistance;

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (!player || !playerCamera) return;

        if (Time.time >= nextUpdateTime)
        {
            isVisible = CheckVisibility();
            
            // Force chase if too far
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > forceChaseDistance)
            {
                isVisible = false; 
            }

            nextUpdateTime = Time.time + updateRate;
        }

        if (isVisible)
        {
            // FREEZE
            agent.isStopped = true;
            agent.velocity = Vector3.zero; 
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            // RUN
            agent.isStopped = false;
            agent.SetDestination(player.position);
            agent.speed = runSpeed;
            animator.SetFloat("Speed", 1f);
        }
    }

    bool CheckVisibility()
    {
        if (ghostRenderer != null && !ghostRenderer.isVisible) return false;

        Vector3 dirToGhost = (transform.position - playerCamera.position).normalized;
        float distance = Vector3.Distance(transform.position, playerCamera.position);
        float angleToGhost = Vector3.Angle(playerCamera.forward, dirToGhost);

        // --- NEW: DYNAMIC ANGLE CALCULATION ---
        // Lerp angle based on distance. 
        // If dist is 0, angle is min. If dist is >= angleReductionDistance, angle is max.
        float currentDetectionAngle = Mathf.Lerp(minDetectionAngle, maxDetectionAngle, distance / angleReductionDistance);
        
        // Check against dynamic angle
        if (angleToGhost > currentDetectionAngle) return false;

        // Linecast check (ignoring Player layer if configured correctly)
        Vector3 targetPoint = transform.position + Vector3.up * 1.5f; 
        if (Physics.Linecast(playerCamera.position, targetPoint, viewBlockerLayer))
        {
            return false; 
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (!playerCamera) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward * 5f);
        
        Gizmos.color = Color.red;
        if (player != null) Gizmos.DrawWireSphere(player.position, forceChaseDistance);
        
        // Visualizing the reduction range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, angleReductionDistance);
    }
}
