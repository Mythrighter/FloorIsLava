using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    [HideInInspector] public bool isOutOfPlace = false;
    [HideInInspector] public bool isHeldByPlayer = false;

    public Vector3 correctPosition;
    public Quaternion correctRotation;
    public Transform dropPoint;

    private Rigidbody rb;
    private bool beingCarried = false;
    private Transform carryZone;
    private float carrySpeed = 10f;
    private bool isTargeted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        correctPosition = transform.position; //original location
        correctRotation = transform.rotation; //original rotation
    }

    //void Start()
    //{
    //    if (!correctPositionInitialized)
    //    {
    //        correctPosition = transform.position;
    //        correctPositionInitialized = true;
    //    }
    //}

    //public void SetCorrectPosition(Vector3 pos)
    //{
    //    correctPosition = pos;
    //    correctPositionInitialized = true;
    //}

    public void SetTargeted(bool targeted)
    {
        isTargeted = targeted;
    }

    void FixedUpdate()
    {
        if (beingCarried && carryZone != null)
        {
            Vector3 targetPos = carryZone.position;
            Vector3 newPos = Vector3.Lerp(rb.position, targetPos, Time.fixedDeltaTime * carrySpeed);
            rb.MovePosition(newPos);
        }
    }

    void Update()
    {
        if (!isTargeted && !isHeldByPlayer)
            isOutOfPlace = Vector3.Distance(transform.position, correctPosition) > 0.5f;
    }

    public bool NeedsAttention()
    {
        return isOutOfPlace && !isTargeted && !isHeldByPlayer;
    }

    public void StartBeingCarried(Transform carryZone)
    {
        this.carryZone = carryZone;
        beingCarried = true;
        rb.isKinematic = true;
    }

    public void StopBeingCarried()
    {
        beingCarried = false;
        carryZone = null;
        rb.isKinematic = false;
    }

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
            t += Time.deltaTime * 2f;
            rb.MovePosition(Vector3.Lerp(startPos, correctPosition, t));
            yield return null;
        }
        rb.MovePosition(correctPosition);
        isOutOfPlace = false;
        isTargeted = false; // clear targeted once restored
    }
}