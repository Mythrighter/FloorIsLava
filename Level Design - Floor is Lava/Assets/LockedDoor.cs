using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float openAngle = -85f;
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
    private bool isOpen = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.AngleAxis(openAngle, hingeAxis);
    }

    public void HandleClick(Vector3 clickedPosition)
    {
        if (player != null && Vector3.Distance(player.position, clickedPosition) > maxClickDistance)
            return;

        Debug.Log("Door Clicked!");

        if (isLocked)
        {
            PlayerInventory inventory = player != null ? player.GetComponent<PlayerInventory>() : null;

            if (inventory == null)
            {
                Debug.Log("No inventory found on player");
                return;
            }

            if (!inventory.HasKey(requiredKeyId))
            {
                Debug.Log("The door is locked. You need: " + requiredKeyId);
                return;
            }

            //Unlocked
            isLocked = false;
            //if (consumeKeyOnUse) inventory.RemoveKey(requiredKeyId);
            Debug.Log("Door is unlocked with " + requiredKeyId);
        }

        isOpen = !isOpen;
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }
}
