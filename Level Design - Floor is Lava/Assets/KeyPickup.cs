using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static bool hasKey = false;

    void Update()
    {
        PickUp pickup = GetComponent<PickUp>();
        if (pickup != null && pickup.isHolding)
        {
            hasKey = true;
        }
    }
}