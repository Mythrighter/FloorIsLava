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

    [Header("Player Chasing")]
    [Tooltip("The player's Transform.")]
    public Transform player;
    [Tooltip("Radius at which the babysitter triggers the catch sequence.")]
    public float catchDistance = 4f;
    [Tooltip("Movement speed multiplier while chasing.")]
    public float chaseSpeedMultiplier = 1.4f;

    // ── Private ───────────────────────────────────────────────────────────────

    private NavMeshAgent navAgent;
    private float baseSpeed;

    private Item currentTarget;
    private bool carryingItem = false;
    private bool isBusy = false;
    private float waitTime;
    private Vector3 cachedDropPoint;

    private float stuckTimer = 0f;
    private const float stuckTimeout = 4f;

    private CatchSequence catchSequence;
    private PlayerMisbehavior misbehavior;

    private bool catchTriggered = false;

    private enum BabysitterState
    {
        Patrolling,
        MovingToItem,
        MovingToDropPoint,
        ChasingPlayer
    }

    private BabysitterState currentState = BabysitterState.Patrolling;

    // ── Unity Lifecycle ───────────────────────────────────────────────────────

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        baseSpeed = navAgent.speed;

        if (player != null)
        {
            catchSequence = player.GetComponent<CatchSequence>();
            misbehavior = player.GetComponent<PlayerMisbehavior>();
        }

        SetNewPatrolDestination();
    }

    void Update()
    {
        if (isBusy) return;

        switch (currentState)
        {
            case BabysitterState.Patrolling:
                PatrolUpdate();
                DecideNextTask();
                break;

            case BabysitterState.MovingToItem:
                DecideNextTask();
                if (currentState == BabysitterState.MovingToItem)
                    MovingToItemUpdate();
                break;

            case BabysitterState.MovingToDropPoint:
                MovingToDropPointUpdate();
                break;

            case BabysitterState.ChasingPlayer:
                ChasePlayerUpdate();
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
            {
                stuckTimer = 0f;
                SetNewPatrolDestination();
            }
        }
        else
        {
            bool pathIsInvalid = navAgent.pathStatus == NavMeshPathStatus.PathPartial
                              || navAgent.pathStatus == NavMeshPathStatus.PathInvalid;

            if (pathIsInvalid || (navAgent.hasPath && navAgent.velocity.sqrMagnitude < 0.01f))
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= stuckTimeout)
                {
                    Debug.LogWarning("Babysitter stuck — picking a new patrol point.");
                    stuckTimer = 0f;
                    SetNewPatrolDestination();
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }
    }

    void SetNewPatrolDestination()
    {
        if (patrolPoints.Length == 0) return;

        int randomIndex = Random.Range(0, patrolPoints.Length);
        navAgent.SetDestination(patrolPoints[randomIndex].transform.position);
        waitTime = Random.Range(waitTimeMin, waitTimeMax);
    }

    // ── Task Decision (closest task wins) ────────────────────────────────────

    void DecideNextTask()
    {
        bool playerVisible = PlayerIsVisiblyMisbehaving();
        bool itemAvailable = !carryingItem && currentTarget == null;

        float distToPlayer = playerVisible
            ? Vector3.Distance(transform.position, player.position)
            : float.MaxValue;

        Item visibleItem = itemAvailable ? visionCone.FindVisibleTarget() : null;
        float distToItem = visibleItem != null
            ? Vector3.Distance(transform.position, visibleItem.transform.position)
            : float.MaxValue;

        if (!playerVisible && visibleItem == null) return;

        if (playerVisible && distToPlayer <= distToItem)
        {
            BeginChasingPlayer();
        }
        else if (visibleItem != null)
        {
            if (currentState != BabysitterState.MovingToItem || currentTarget != visibleItem)
            {
                currentTarget = visibleItem;
                currentTarget.SetTargeted(true);
                cachedDropPoint = currentTarget.dropPoint != null
                    ? currentTarget.dropPoint.position
                    : currentTarget.correctPosition;
                SetNormalSpeed();
                currentState = BabysitterState.MovingToItem;
            }
        }
    }

    // ── Player Visibility Check ───────────────────────────────────────────────

    bool PlayerIsVisiblyMisbehaving()
    {
        if (player == null || misbehavior == null) return false;
        if (!misbehavior.IsMisbehaving) return false;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 targetPos = player.position + Vector3.up * 0.5f;
        Vector3 dir = (targetPos - rayOrigin).normalized;
        float dist = Vector3.Distance(rayOrigin, targetPos);
        float angle = Vector3.Angle(transform.forward, dir);

        bool inCone = angle < visionCone.viewAngle / 2f && dist <= visionCone.viewRadius;
        bool inAwareness = dist <= visionCone.awarenessRadius;

        if (!inCone && !inAwareness) return false;

        int playerLayer = player.gameObject.layer;
        int losMask = visionCone.obstacleMask & ~(1 << playerLayer);

        bool blocked = Physics.Raycast(rayOrigin, dir, dist, losMask);
        return !blocked;
    }

    // ── Chase Player ──────────────────────────────────────────────────────────

    void BeginChasingPlayer()
    {
        if (currentTarget != null)
        {
            currentTarget.SetTargeted(false);
            currentTarget = null;
        }

        navAgent.isStopped = false;
        navAgent.speed = baseSpeed * chaseSpeedMultiplier;
        currentState = BabysitterState.ChasingPlayer;
    }

    void ChasePlayerUpdate()
    {
        if (!PlayerIsVisiblyMisbehaving())
        {
            AbortChase();
            return;
        }

        navAgent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        if (!catchTriggered && dist <= catchDistance)
        {
            catchTriggered = true;
            catchSequence?.TriggerCatch(transform, navAgent);
        }
    }

    void AbortChase()
    {
        if (catchTriggered) return;

        navAgent.isStopped = false;
        navAgent.ResetPath();
        SetNormalSpeed();
        ReturnToPatrol();
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

        if (dist <= interactionDistance && !isBusy)
            StartCoroutine(PickUpItem());
    }

    IEnumerator PickUpItem()
    {
        isBusy = true;
        navAgent.isStopped = true;

        yield return new WaitForSeconds(pickupDropDelay);

        if (currentTarget == null)
        {
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

        if (dist <= interactionDistance && !isBusy)
            StartCoroutine(DropItem());
    }

    IEnumerator DropItem()
    {
        isBusy = true;
        navAgent.isStopped = true;

        yield return new WaitForSeconds(pickupDropDelay);

        carryingItem = false;
        currentTarget.StopBeingCarried();

        currentTarget.transform.position = currentTarget.correctPosition;
        currentTarget.transform.rotation = currentTarget.correctRotation;
        currentTarget.isOutOfPlace = false;
        currentTarget.SetTargeted(false);

        currentTarget = null;
        ReturnToPatrol();

        navAgent.isStopped = false;
        isBusy = false;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    void ReturnToPatrol()
    {
        if (currentTarget != null)
        {
            currentTarget.SetTargeted(false);
            currentTarget = null;
        }

        currentState = BabysitterState.Patrolling;
        SetNewPatrolDestination();
    }

    void SetNormalSpeed()
    {
        navAgent.speed = baseSpeed;
    }

    /// <summary>
    /// Called by CatchSequence when the escape timeout fires.
    /// Resets catch state and sends the babysitter back to patrolling.
    /// </summary>
    public void ResumeAfterFailedCatch()
    {
        catchTriggered = false;
        SetNormalSpeed();
        navAgent.isStopped = false;
        ReturnToPatrol();
    }
}