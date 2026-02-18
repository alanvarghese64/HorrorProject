using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    public float maxDetectionAngle = 60f;
    public float minDetectionAngle = 20f;
    public float angleReductionDistance = 5f;

    [Header("Aggression")]
    public float forceChaseDistance = 15f;

    [Header("Visibility Logic")]
    public LayerMask viewBlockerLayer;
    
    // ==================== NEW: JUMP ATTACK ====================
    [Header("Jump Attack")]
    public float jumpAttackRange = 2.5f;
    public float jumpAttackCooldown = 5f;
    public string jumpAttackAnimName = "JumpAttack";
    
    private bool canJumpAttack = true;
    private bool isJumpAttacking = false;
    // ==========================================================
    
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
            // ==================== NEW: CHECK FOR JUMP ATTACK ====================
            if (!isJumpAttacking)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                
                if (distanceToPlayer <= jumpAttackRange && canJumpAttack)
                {
                    StartCoroutine(PerformJumpAttack());
                    return;
                }
            }
            // ====================================================================

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

        float currentDetectionAngle = Mathf.Lerp(minDetectionAngle, maxDetectionAngle, distance / angleReductionDistance);
        
        if (angleToGhost > currentDetectionAngle) return false;

        Vector3 targetPoint = transform.position + Vector3.up * 1.5f; 
        if (Physics.Linecast(playerCamera.position, targetPoint, viewBlockerLayer))
        {
            return false; 
        }

        return true;
    }

    // ==================== NEW: JUMP ATTACK COROUTINE ====================
    IEnumerator PerformJumpAttack()
    {
        isJumpAttacking = true;
        canJumpAttack = false;
        
        // Stop and face player
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Play jump attack animation
        if (animator != null)
        {
            animator.Play(jumpAttackAnimName);
        }
        
        // Wait for animation to finish (adjust this time to match your animation length)
        yield return new WaitForSeconds(1.5f);
        
        // Resume chasing
        isJumpAttacking = false;
        agent.isStopped = false;
        
        // Cooldown before next attack
        yield return new WaitForSeconds(jumpAttackCooldown);
        canJumpAttack = true;
    }
    // ====================================================================

    void OnDrawGizmosSelected()
    {
        if (!playerCamera) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward * 5f);
        
        Gizmos.color = Color.red;
        if (player != null) Gizmos.DrawWireSphere(player.position, forceChaseDistance);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, angleReductionDistance);
        
        // ==================== NEW: SHOW ATTACK RANGE ====================
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, jumpAttackRange);
        // ================================================================
    }
}
