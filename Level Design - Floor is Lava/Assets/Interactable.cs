using TMPro.Examples;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    [SerializeField] private GameObject movePrompt;

    private void Start()
    {
        if (movePrompt != null)
            movePrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && movePrompt != null)
            movePrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && movePrompt != null)
            movePrompt.SetActive(false);
    }

    private void PickUp()
    {
        Debug.Log("Moved " + gameObject.name);
    }
}
