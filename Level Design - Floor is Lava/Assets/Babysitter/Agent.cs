using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Agent : MonoBehaviour
{
    [Header("Patrol Settings")]
    public GameObject[] patrolPoints;
    public float waitTimeMin = 10f;
    public float waitTimeMax = 30f;

    [Header("Item Detection")]
    public VisionCone visionCone;
    public float interactionDistance = 1.5f; // Distance for pickup/drop

    [Header("Carry Settings")]
    public Transform carryZone; // Empty transform at hand height
    public float pickupDropDelay = 1f; // Wait time at pickup/drop

    private NavMeshAgent navAgent;
    private float waitTime;

    private Item currentTarget;
    private bool carryingItem = false;
    private bool isBusy = false; // Prevent actions while waiting

    private enum AgentState { Patrolling, MovingToTarget, CleaningUp }
    private AgentState currentState = AgentState.Patrolling;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        SetNewPatrol();
    }

    void Update()
    {
        if (isBusy) return; // skip updates while paused

        switch (currentState)
        {
            case AgentState.Patrolling:
                PatrolUpdate();
                ScanForItems();
                break;

            case AgentState.MovingToTarget:
                MoveToTargetUpdate();
                break;
        }
    }

    #region Patrol
    void PatrolUpdate()
    {
        waitTime -= Time.deltaTime;

        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            if (waitTime <= 0)
                SetNewPatrol();
        }
    }

    void SetNewPatrol()
    {
        if (patrolPoints.Length == 0) return;

        int randomIndex = Random.Range(0, patrolPoints.Length);
        navAgent.SetDestination(patrolPoints[randomIndex].transform.position);
        waitTime = Random.Range(waitTimeMin, waitTimeMax);
    }
    #endregion

    #region Item Detection
    void ScanForItems()
    {
        if (carryingItem) return; // Already carrying, ignore

        Item target = visionCone.FindVisibleTarget();
        if (target != null)
        {
            currentTarget = target;
            currentState = AgentState.MovingToTarget;
        }
    }
    #endregion

    #region Moving To Target
    void MoveToTargetUpdate()
    {
        if (currentTarget == null)
        {
            currentState = AgentState.Patrolling;
            carryingItem = false;
            return;
        }

        // Determine destination
        Vector3 destination = carryingItem ? currentTarget.correctPosition : currentTarget.transform.position;
        navAgent.SetDestination(destination);

        // Distance check: carryZone for pickup, agent position for drop
        float dist = carryingItem
            ? Vector3.Distance(transform.position, currentTarget.correctPosition)
            : Vector3.Distance(carryZone.position, currentTarget.transform.position);

        if (dist <= interactionDistance && !isBusy)
        {
            if (!carryingItem)
            {
                StartCoroutine(PickUpItem());
            }
            else
            {
                StartCoroutine(DropItem());
            }
        }
    }

    IEnumerator PickUpItem()
    {
        isBusy = true;
        navAgent.isStopped = true; // stop movement while picking up

        yield return new WaitForSeconds(pickupDropDelay);

        carryingItem = true;
        currentTarget.StartBeingCarried(carryZone);

        if (currentTarget != null)
        {
            currentState = AgentState.CleaningUp;
        }

        navAgent.isStopped = false;
        isBusy = false;
    }

    IEnumerator DropItem()
    {
        isBusy = true;
        navAgent.isStopped = true; // stop movement while dropping

        yield return new WaitForSeconds(pickupDropDelay);

        carryingItem = false;
        currentTarget.StopBeingCarried();
        currentTarget.Restore();

        currentTarget = null;
        currentState = AgentState.Patrolling;

        navAgent.isStopped = false;
        isBusy = false;
    }
    #endregion
}