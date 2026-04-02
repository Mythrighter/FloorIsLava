using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 120f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Debug")]
    public bool drawGizmos = true;

    public Item FindVisibleTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        Item bestTarget = null;

        foreach (Collider col in targets)
        {
            Item item = col.GetComponent<Item>();
            if (item == null || !item.NeedsAttention()) continue;

            Vector3 dirToTarget = (item.transform.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, item.transform.position);

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, dist, obstacleMask))
                {
                    bestTarget = item;
                    break;
                }
            }
        }

        return bestTarget;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward;
        Quaternion leftRot = Quaternion.Euler(0, -viewAngle / 2, 0);
        Quaternion rightRot = Quaternion.Euler(0, viewAngle / 2, 0);
        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.DrawRay(transform.position, leftDir * viewRadius);
        Gizmos.DrawRay(transform.position, rightDir * viewRadius);
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}