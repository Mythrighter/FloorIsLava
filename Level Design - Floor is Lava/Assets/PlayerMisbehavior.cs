using UnityEngine;

/// <summary>
/// Attach to the Player. Tracks whether the player is currently doing something
/// "wrong" that the babysitter should react to.
///
/// Bad behaviours:
///   - Standing on a CouchCushion, Table, or Stove  (detected via trigger)
///   - Holding a CouchCushion, BigCushion, or Bowl   (reported by PickUp)
/// </summary>
public class PlayerMisbehavior : MonoBehaviour
{
    public static PlayerMisbehavior Instance { get; private set; }

    private int triggerCount = 0;
    private bool holdingBadItem = false;

    public bool IsMisbehaving => triggerCount > 0 || holdingBadItem;

    void Awake()
    {
        Instance = this;
    }

    public void SetHoldingBadItem(bool value)
    {
        holdingBadItem = value;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsBadSurface(other.gameObject))
            triggerCount++;
    }

    void OnTriggerExit(Collider other)
    {
        if (IsBadSurface(other.gameObject))
            triggerCount = Mathf.Max(0, triggerCount - 1);
    }

    private bool IsBadSurface(GameObject go)
    {
        return go.CompareTag("CouchCushion")
            || go.CompareTag("Table")
            || go.CompareTag("Stove");
    }
}