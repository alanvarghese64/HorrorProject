using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EmployeeController : MonoBehaviour
{
    // --- THIS WAS MISSING ---
    public EmployeeData data; 
    // ------------------------

    [Header("Configuration")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3.5f;

    // References
    private NavMeshAgent agent;
    private Animator animator;
    
    // Action Queue
    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();
    private bool isBusy = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // Ensure Agent controls rotation
        agent.updateRotation = true; 
    }

    private void Update()
    {
    // OLD LOGIC (Deleted):
        // float currentSpeed = agent.velocity.magnitude;
        // animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);

        // NEW LOGIC (Explicit States):
        // Check if we are moving significantly
        bool isMoving = agent.velocity.magnitude > 0.1f;
        
        // Update the Animator Bool
        animator.SetBool("IsWalking", isMoving);
        
        // If you want running logic later, you would set IsRunning here based on agent.speed
        // Example: animator.SetBool("IsRunning", isMoving && agent.speed > 3f);

        // 2. ACTION SYSTEM
        if (!isBusy && actionQueue.Count > 0)
        {
            StartCoroutine(ProcessNextAction());
        }
    }

    private IEnumerator ProcessNextAction()
    {
        isBusy = true;
        IEnumerator action = actionQueue.Dequeue();
        yield return StartCoroutine(action);
        isBusy = false;
    }

    // --- PUBLIC METHODS ---
    
    public void GoToLocation(Vector3 pos)
    {
        actionQueue.Enqueue(MoveRoutine(pos));
    }

    public void DoWorkEffect(float duration)
    {
        actionQueue.Enqueue(AnimationRoutine("Trigger_Work", duration));
    }

    public void GetScaredEffect(float duration)
    {
        actionQueue.Clear(); 
        isBusy = true; 
        StartCoroutine(PanicRoutine(duration));
    }
     public void SitOnChair(Transform chairTransform)
    {
        actionQueue.Enqueue(SitRoutine(chairTransform));
    }

    public void StandUpFromChair()
    {
        actionQueue.Enqueue(StandUpRoutine());
    }

    // --- INTERNAL ROUTINES ---

    private IEnumerator MoveRoutine(Vector3 destination)
    {
        agent.speed = walkSpeed;
        agent.SetDestination(destination);
        yield return null; 

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }
    }

    private IEnumerator AnimationRoutine(string trigger, float duration)
    {
        agent.isStopped = true;
        animator.SetTrigger(trigger);
        yield return new WaitForSeconds(duration);
        animator.SetTrigger("Trigger_Stop");
        agent.isStopped = false;
    }

    private IEnumerator SitRoutine(Transform chair)
    {
        // 1. Walk to the chair
        agent.speed = walkSpeed;
        agent.SetDestination(chair.position);
        yield return null;

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // 2. Align to face the chair (important for natural sitting)
        Vector3 direction = chair.forward; // Chair's forward is where he should face
        direction.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(direction);

        float time = 0;
        while (time < 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, time * 4f);
            time += Time.deltaTime;
            yield return null;
        }

        // 3. Stop the NavMeshAgent (crucial - otherwise he'll keep trying to stand on the exact chair position)
        agent.isStopped = true;

        // 4. Snap position slightly (optional - to perfectly align with chair seat)
        transform.position = chair.position;

        // 5. Play Sit Animation
        animator.SetBool("IsSitting", true);
        animator.SetTrigger("Trigger_Sit");
    }

    private IEnumerator StandUpRoutine()
    {
        // 1. Trigger Stand Up
        animator.SetTrigger("Trigger_StandUp");
        animator.SetBool("IsSitting", false);

        // 2. Wait for animation to finish
        yield return new WaitForSeconds(1.5f); // Adjust based on your animation length

        // 3. Re-enable movement
        agent.isStopped = false;
    }

    private IEnumerator PanicRoutine(float duration)
    {
        agent.isStopped = true;
        animator.SetTrigger("Trigger_Scared");
        yield return new WaitForSeconds(duration);
        animator.SetTrigger("Trigger_Stop");
        agent.isStopped = false;
        isBusy = false;
    }

        public void RunToLocation(Vector3 pos)
    {
        // Queue a routine that sets speed high and toggles Run bool
        actionQueue.Enqueue(RunRoutine(pos));
    }

    private IEnumerator RunRoutine(Vector3 destination)
    {
        agent.speed = runSpeed; // Set physical speed high
        animator.SetBool("IsRunning", true); // Force Run Anim
        
        agent.SetDestination(destination);
        yield return null;

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // Reset when done
        animator.SetBool("IsRunning", false);
        agent.speed = walkSpeed; // Reset to walk speed
    }

}
