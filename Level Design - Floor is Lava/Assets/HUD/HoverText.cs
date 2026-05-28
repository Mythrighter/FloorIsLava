using UnityEngine;
using TMPro;

public class HoverText : MonoBehaviour
{
    public GameObject hoverTextLocked;
    public GameObject hoverTextUnlocked;
    public GameObject hoverTextOpen;
    public GameObject hoverTextClose;
    public LockedDoor door;
    public PlayerInventory inventory;

    private void Start()
    {
        hoverTextLocked.SetActive(false);
        hoverTextUnlocked.SetActive(false);
        hoverTextOpen.SetActive(false);
        hoverTextClose.SetActive(false);
    }

    private void Update()
    {
        if (!hoverTextLocked.activeSelf && !hoverTextUnlocked.activeSelf
            && !hoverTextOpen.activeSelf && !hoverTextClose.activeSelf)
            return;

        UpdateHoverText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            UpdateHoverText();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            hoverTextLocked.SetActive(false);
            hoverTextUnlocked.SetActive(false);
            hoverTextOpen.SetActive(false);
            hoverTextClose.SetActive(false);
        }
    }

    private void UpdateHoverText()
    {
        bool canUnlock = !door.isLocked || inventory.HasKey(door.requiredKeyId);

        // Locked and no key
        hoverTextLocked.SetActive(!canUnlock);

        // Has key but door not yet unlocked
        hoverTextUnlocked.SetActive(canUnlock && door.isLocked);

        // Unlocked and door is closed
        hoverTextOpen.SetActive(!door.isLocked && !door.isOpen);

        // Unlocked and door is open
        hoverTextClose.SetActive(!door.isLocked && door.isOpen);
    }
}