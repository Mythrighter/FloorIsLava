using UnityEngine;

public class PagePickup : MonoBehaviour
{
    [Tooltip("The PanelToggle in your scene.")]
    public DiaryPages diaryPages;

    [Tooltip("The page (UI GameObject or prefab instance) this pickup unlocks.")]
    public GameObject pageToUnlock;

    [Tooltip("Open the panel and jump to the new page the moment it's collected.")]
    public bool showOnPickup = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        diaryPages.AddPage(pageToUnlock, showOnPickup);
        Destroy(gameObject);
    }
}
