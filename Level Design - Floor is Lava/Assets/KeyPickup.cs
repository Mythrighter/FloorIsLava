using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string keyId = "BathroomKey";
    public GameObject inventoryKey; 

    public void Start()
    {
        inventoryKey.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddKey(keyId);
            Debug.Log("Picked up: " + keyId);
            Destroy(gameObject);
            inventoryKey.SetActive(true);
        }
        else
        {
            inventoryKey.SetActive(false);
        }
    }
}