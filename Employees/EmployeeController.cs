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

    [Header("Task System")]
public int currentTaskIndex = 0;
private float taskTimer = 0f;
private bool isWaitingForNextTask = false;
public TaskData currentTask;
public bool isConvinced = false;


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
    // 1. ANIMATION LOGIC (keep existing)
    bool isMoving = agent.velocity.magnitude > 0.1f;
    animator.SetBool("IsWalking", isMoving);

    // 2. ACTION SYSTEM (keep existing)
    if (!isBusy && actionQueue.Count > 0)
    {
        StartCoroutine(ProcessNextAction());
    }
    
    // 3. NEW: Task timing system (add this)
    if (isWaitingForNextTask && !isConvinced)
    {
        taskTimer += Time.deltaTime;
        
        if (taskTimer >= data.timeBetweenTasks)
        {
            taskTimer = 0f;
            isWaitingForNextTask = false;
            StartNextTask();
        }
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

public void StartNextTask()
{
    if (data.assignedTaskSequence == null || data.assignedTaskSequence.Count == 0)
        return;
        
    if (isConvinced)
        return;
    
    if (currentTaskIndex >= data.assignedTaskSequence.Count)
    {
        currentTaskIndex = 0;
    }
    
    currentTask = data.assignedTaskSequence[currentTaskIndex];
    currentTaskIndex++;
    
    // Execute action sequence
    StartCoroutine(ExecuteTaskActionSequence(currentTask));
}


private IEnumerator ExecuteTaskActionSequence(TaskData task)
{
    isBusy = true;
    
    // Execute each action in sequence
    if (task.actionSequence != null && task.actionSequence.Count > 0)
    {
        foreach (EmployeeAction action in task.actionSequence)
        {
            yield return StartCoroutine(ExecuteAction(action));
        }
    }
    
    // Task complete
    CompleteTask(task);
    
    isBusy = false;
    currentTask = null;
    
    // Wait for next task with timer
    isWaitingForNextTask = true;
    taskTimer = 0f;
}

private IEnumerator ExecuteAction(EmployeeAction action)
{
    switch (action.actionType)
    {
        case ActionType.WalkTo:
            if (action.targetLocation != null)
                yield return StartCoroutine(MoveRoutine(action.targetLocation.position));
            break;
            
        case ActionType.RunTo:
            if (action.targetLocation != null)
                yield return StartCoroutine(RunRoutine(action.targetLocation.position));
            break;
            
        case ActionType.Sit:
            if (action.chairTransform != null)
                yield return StartCoroutine(SitRoutine(action.chairTransform));
            break;
            
        case ActionType.StandUp:
            yield return StartCoroutine(StandUpRoutine());
            break;
            
        case ActionType.PlayAnimation:
            yield return StartCoroutine(AnimationRoutine(action.animationTrigger, action.animationDuration));
            break;
            
        case ActionType.Wait:
            yield return new WaitForSeconds(action.waitDuration);
            break;
            
        case ActionType.LookAt:
            if (action.lookAtTarget != null)
                yield return StartCoroutine(LookAtRoutine(action.lookAtTarget));
            break;
    }
}

private IEnumerator LookAtRoutine(Transform target)
{
    Vector3 direction = (target.position - transform.position);
    direction.y = 0;
    
    if (direction.magnitude > 0.1f)
    {
        Quaternion lookRot = Quaternion.LookRotation(direction);
        
        float time = 0;
        while (time < 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, time * 4f);
            time += Time.deltaTime;
            yield return null;
        }
    }
}

private void CompleteTask(TaskData task)
{
    if (task.isDarkTask)
    {
        SpiritManager.Instance.AddPower(task.spiritPowerGain);
        GameEvents.OnDarkTaskStarted?.Invoke(task);
    }
    else
    {
        GameEvents.OnTaskCompleted?.Invoke(task);
    }
}


}
