using UnityEngine;

public class HoverPickUp : MonoBehaviour

{
    public GameObject hoverTextPickUp;

    private void Start()
    {
        hoverTextPickUp.SetActive(false);

    }

    private void Update()
    {
        if (!hoverTextPickUp.activeSelf)
            return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            hoverTextPickUp.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            hoverTextPickUp.SetActive(false);
        }
    }
}