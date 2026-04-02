using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    [HideInInspector] public bool isOutOfPlace = false;

    public Vector3 correctPosition;        // where the item should be
    private Rigidbody rb;

    // Physics-based carry
    private bool beingCarried = false;
    private Transform carryZone;
    private float carrySpeed = 10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Save the initial position if correctPosition not set
        if (correctPosition == Vector3.zero)
            correctPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Follow agent using Rigidbody if being carried
        if (beingCarried && carryZone != null)
        {
            Vector3 targetPos = carryZone.position;
            Vector3 newPos = Vector3.Lerp(rb.position, targetPos, Time.fixedDeltaTime * carrySpeed);
            rb.MovePosition(newPos);
        }
    }

    void Update()
    {
        // Check if item is out of place
        isOutOfPlace = Vector3.Distance(transform.position, correctPosition) > 0.5f;
    }

    public bool NeedsAttention()
    {
        return isOutOfPlace;
    }

    // Called by agent to pick up
    public void StartBeingCarried(Transform carryZone)
    {
        this.carryZone = carryZone;
        beingCarried = true;
    }

    // Called by agent to stop carrying
    public void StopBeingCarried()
    {
        beingCarried = false;
        carryZone = null;
    }

    // Smoothly restore to original location
    public void Restore()
    {
        StartCoroutine(MoveToCorrectPosition());
    }

    private IEnumerator MoveToCorrectPosition()
    {
        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // speed multiplier
            rb.MovePosition(Vector3.Lerp(startPos, correctPosition, t));
            yield return null;
        }

        rb.MovePosition(correctPosition);
        isOutOfPlace = false;
    }
}