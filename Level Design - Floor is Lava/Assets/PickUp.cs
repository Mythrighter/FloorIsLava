using Unity.Cinemachine;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    bool isHolding = false;

    [SerializeField] float throwForce = 300;
    [SerializeField] float maxDistance = 3f;

    float distance;
    TempParent tempParent;
    Rigidbody rb;
    Vector3 objectPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tempParent = TempParent.Instance;
    }

    void Update()
    {
        if (isHolding)
            Hold();
    }

    void OnMouseDown()
    {
        if (tempParent == null)
        {
            Debug.Log("Temp Parent item not found in scene");
            return;
        }

        distance = Vector3.Distance(transform.position, tempParent.transform.position);
        if (distance > maxDistance) return;

        isHolding = true;
        rb.useGravity = false;
        rb.detectCollisions = true;
        transform.SetParent(tempParent.transform);

        Item item = GetComponent<Item>();
        if (item != null) item.isHeldByPlayer = true;

        // Score + notify PlayerMisbehavior if it's a bad item
        bool isBadItem = false;

        if (CompareTag("CouchCushion"))
        {
            ScoreManager.sManager.IncreaseScoreCouchCushion(10);
            isBadItem = true;
        }
        if (CompareTag("BigCushion"))
        {
            ScoreManager.sManager.IncreaseScoreBigCushion(20);
            isBadItem = true;
        }
        if (CompareTag("Bowl"))
        {
            ScoreManager.sManager.IncreaseScoreBowl(5);
            isBadItem = true;
        }

        if (isBadItem && PlayerMisbehavior.Instance != null)
            PlayerMisbehavior.Instance.SetHoldingBadItem(true);
    }

    private void OnMouseUp() => Drop();
    private void OnMouseExit() => Drop();

    private void Hold()
    {
        distance = Vector3.Distance(transform.position, tempParent.transform.position);
        if (distance >= maxDistance) { Drop(); return; }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (Input.GetMouseButtonDown(1))
        {
            rb.AddForce(tempParent.transform.forward * throwForce);
            Drop();
        }
    }

    private void Drop()
    {
        if (!isHolding) return;

        isHolding = false;
        objectPos = transform.position;
        transform.position = objectPos;
        transform.SetParent(null);
        rb.useGravity = true;

        Item item = GetComponent<Item>();
        if (item != null) item.isHeldByPlayer = false;

        // Always clear the bad item flag on drop, regardless of tag
        if (PlayerMisbehavior.Instance != null)
            PlayerMisbehavior.Instance.SetHoldingBadItem(false);
    }
}