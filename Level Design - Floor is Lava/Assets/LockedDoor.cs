using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float openAngle = 170f;
    public float speed = 2f;
    public Vector3 hingeAxis = Vector3.up;

    [Header("Interaction")]
    public float maxClickDistance = 3f;
    public Transform player;

    [Header("Lock Settings")]
    public bool isLocked = true;
    public string requiredKeyId = "BathroomKey"; //bathroom key opens this door
    public bool consumeKeyOnUse = false; //remove key from inventory after use

    private Quaternion closedRotation;
    private Quaternion openRotation;
    public bool isOpen = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.AngleAxis(openAngle, hingeAxis);
    }

    public void HandleClick(Vector3 clickedPosition)
    {
        Debug.Log("HandleClick received");

        if (player != null && Vector3.Distance(player.position, clickedPosition) > maxClickDistance)
        {
            Debug.Log($"Too far: distance is {Vector3.Distance(player.position, clickedPosition)}");
            return;
        }

        Debug.Log($"Is locked: {isLocked}, Player null: {player == null}");

        if (isLocked)
        {
            PlayerInventory inventory = player != null ? player.GetComponent<PlayerInventory>() : null;
            Debug.Log($"Inventory null: {inventory == null}");

            if (inventory == null)
            {
                Debug.Log("No inventory found on player");
                return;
            }

            bool hasKey = inventory.HasKey(requiredKeyId);
            Debug.Log($"Has key '{requiredKeyId}': {hasKey}");

            if (!hasKey)
            {
                Debug.Log("Key not found in inventory");
                return;
            }

            isLocked = false;
            Debug.Log("Door unlocked with " + requiredKeyId);
        }

        isOpen = !isOpen;
        Debug.Log($"Door is now {(isOpen ? "open" : "closed")}");
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }
}
