using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string keyId = "default";
    [SerializeField] private Sprite inventoryIcon;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddKey(keyId, inventoryIcon);
        }

        Destroy(gameObject);
    }
}