using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public GameObject winCanvas;
    public static bool GameIsPaused = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Gate triggered by: " + other.name + " | hasKey: " + KeyPickup.hasKey);
        if (KeyPickup.hasKey)
        {
            winCanvas.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}