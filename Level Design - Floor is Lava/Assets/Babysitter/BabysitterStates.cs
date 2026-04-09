using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro.Examples;

public class Babysitter : MonoBehaviour
{
    [Header("Patrol Settings")]
    public GameObject[] patrolPoints;
    public float waitTimeMin = 10f;
    public float waitTimeMax = 30f;

    [Header("Item Detection")]
    public VisionCone visionCone;
    public float interactionDistance = 1.5f;

    [Header("Carry Settings")]
    public Transform carryZone;
    public float pickupDropDelay = 1f;

    private NavMeshAgent navAgent;
    private Item currentTarget;
    private bool carryingItem = false;
    private bool isBusy = false;
    private float waitTime;
    private Vector3 cachedDropPoint;

    private enum BabysitterState { Patrolling, MovingToItem, MovingToDropPoint }
    private BabysitterState currentState = BabysitterState.Patrolling;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        SetNewPatrolDestination();
    }

    void Update()
    {
        if (isBusy) return;

        Debug.Log("Current state: " + currentState);


        switch (currentState)
        {
            case BabysitterState.Patrolling:
                PatrolUpdate();
                ScanForItems();
                break;

            case BabysitterState.MovingToItem:
                MovingToItemUpdate();
                break;

            case BabysitterState.MovingToDropPoint:
                MovingToDropPointUpdate();
                break;
        }
    }

    // ── Patrol ────────────────────────────────────────────────────────────────

    void PatrolUpdate()
    {
        waitTime -= Time.deltaTime;

        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            if (waitTime <= 0)
                SetNewPatrolDestination();
        }
    }

    void SetNewPatrolDestination()
    {
        if (patrolPoints.Length == 0) return;

        int randomIndex = Random.Range(0, patrolPoints.Length);
        navAgent.SetDestination(patrolPoints[randomIndex].transform.position);
        waitTime = Random.Range(waitTimeMin, waitTimeMax);
    }

    // ── Item Detection ────────────────────────────────────────────────────────

    void ScanForItems()
    {
        if (carryingItem) return;

        Item target = visionCone.FindVisibleTarget();

        if (target != null)
        {
            currentTarget = target;
            currentTarget.SetTargeted(true);
            cachedDropPoint = currentTarget.dropPoint != null
                ? currentTarget.dropPoint.position
                : currentTarget.correctPosition;
            currentState = BabysitterState.MovingToItem;
        }
    }

    // ── Moving To Item ────────────────────────────────────────────────────────

    void MovingToItemUpdate()
    {
        if (currentTarget == null)
        {
            ReturnToPatrol();
            return;
        }

        navAgent.SetDestination(currentTarget.transform.position);

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
        Debug.Log("Distance to item: " + dist + " | isBusy: " + isBusy);

        if (dist <= interactionDistance && !isBusy)
        {
            StartCoroutine(PickUpItem());
        }
    }

    IEnumerator PickUpItem()
    {
        Debug.Log("PickUpItem started");
        isBusy = true;
        navAgent.isStopped = true;

        yield return new WaitForSeconds(pickupDropDelay);

        Debug.Log("PickUpItem completed");

        if (currentTarget == null)
        {
            Debug.Log("currentTarget is null after wait!");
            isBusy = false;
            navAgent.isStopped = false;
            ReturnToPatrol();
            yield break;
        }

        carryingItem = true;
        currentTarget.StartBeingCarried(carryZone);
        currentState = BabysitterState.MovingToDropPoint;

        navAgent.isStopped = false;
        isBusy = false;
    }

    // ── Moving To Drop Point ──────────────────────────────────────────────────

    void MovingToDropPointUpdate()
    {
        if (currentTarget == null)
        {
            carryingItem = false;
            ReturnToPatrol();
            return;
        }

        navAgent.SetDestination(cachedDropPoint);

        float dist = Vector3.Distance(transform.position, cachedDropPoint);
        Debug.Log("Distance to drop point: " + dist + " | cachedDropPoint: " + cachedDropPoint + " | isStopped: " + navAgent.isStopped + " | hasPath: " + navAgent.hasPath);

        if (dist <= interactionDistance && !isBusy)
        {
            StartCoroutine(DropItem());
        }
    }

    IEnumerator DropItem()
    {
        isBusy = true;
        navAgent.isStopped = true;

        yield return new WaitForSeconds(pickupDropDelay);

        carryingItem = false;
        currentTarget.StopBeingCarried();

        // Instantly teleport to correct position
        currentTarget.transform.position = currentTarget.correctPosition;
        currentTarget.transform.rotation = currentTarget.correctRotation;
        currentTarget.isOutOfPlace = false;

        currentTarget = null;
        ReturnToPatrol();

        navAgent.isStopped = false;
        isBusy = false;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    void ReturnToPatrol()
    {
        if(currentTarget != null)
        {
            currentTarget.SetTargeted(false);
            currentTarget = null;
        }

        currentState = BabysitterState.Patrolling;
        SetNewPatrolDestination();
    }
}