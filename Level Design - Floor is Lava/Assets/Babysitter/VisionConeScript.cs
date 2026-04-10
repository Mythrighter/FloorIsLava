using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 120f;
    public float viewHeight = 4f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public float awarenessRadius = 4f;

    [Header("Debug")]
    public bool drawGizmos = true;

    public Item FindVisibleTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        Debug.Log("Items in range: " + targets.Length);

        Item bestTarget = null;
        foreach (Collider col in targets)
        {
            Item item = col.GetComponentInParent<Item>();
            if (item == null)
            {
                Debug.Log(col.name + " skipped - no Item component");
                continue;
            }
            if (!item.NeedsAttention())
            {
                Debug.Log(col.name + " skipped - NeedsAttention false");
                continue;
            }

            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            Vector3 targetPos = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
            Vector3 dirToTarget = (targetPos - rayOrigin).normalized;
            float dist = Vector3.Distance(rayOrigin, targetPos);
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            Debug.Log(col.name + " | angle: " + angle + " | viewAngle/2: " + (viewAngle / 2) + " | dist: " + dist);

            bool inVisionCone = angle < viewAngle / 2;
            bool inAwarenessRadius = dist <= awarenessRadius;

            Debug.Log(col.name + " | inVisionCone: " + inVisionCone + " | inAwarenessRadius: " + inAwarenessRadius);

            if (inVisionCone || inAwarenessRadius)
            {
                int combinedMask = obstacleMask & ~targetMask;
                RaycastHit rayHit;
                bool blocked = Physics.Raycast(rayOrigin, dirToTarget, out rayHit, dist, combinedMask);
                Debug.Log(col.name + " | blocked: " + blocked + " | hit object: " + (blocked ? rayHit.collider.gameObject.name : "nothing"));

                if (!blocked)
                {
                    bestTarget = item;
                    break;
                }
            }
            else
            {
                Debug.Log(col.name + " skipped - outside cone and awareness radius");
            }
        }
        return bestTarget;
    }

    private void OnDrawGizmos()
    {     
        if (!drawGizmos) return;

        //Vision cone
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward;
        Quaternion leftRot = Quaternion.Euler(-viewHeight, -viewAngle / 2, 0);
        Quaternion rightRot = Quaternion.Euler(-viewHeight, viewAngle / 2, 0);
        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;
        Gizmos.DrawRay(transform.position, leftDir * viewRadius);
        Gizmos.DrawRay(transform.position, rightDir * viewRadius);
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        //Awareness radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, awarenessRadius);
    }
}