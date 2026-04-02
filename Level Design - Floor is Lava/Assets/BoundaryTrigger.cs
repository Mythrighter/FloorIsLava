using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoundaryTrigger : MonoBehaviour
{
    [Header("Optional Settings")]
    public float respawnDelay = 0f; // seconds before respawn

    // Dictionary to store the original transforms of all objects in the scene
    private Dictionary<GameObject, (Vector3 position, Quaternion rotation)> originalTransforms
        = new Dictionary<GameObject, (Vector3, Quaternion)>();

    private void Start()
    {
        // Find all objects with a collider in the scene
        Collider[] allObjects = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (Collider col in allObjects)
        {
            GameObject obj = col.gameObject;
            // Only store each object once
            if (!originalTransforms.ContainsKey(obj))
            {
                originalTransforms[obj] = (obj.transform.position, obj.transform.rotation);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        // Only respawn objects that we know the original transform for
        if (!originalTransforms.ContainsKey(obj)) return;

        if (respawnDelay > 0)
            StartCoroutine(RespawnAfterDelay(obj, respawnDelay));
        else
            Respawn(obj);
    }

    private void Respawn(GameObject obj)
    {
        if (!originalTransforms.ContainsKey(obj)) return;

        var (pos, rot) = originalTransforms[obj];

        Destroy(obj);

        GameObject newObj = Instantiate(obj, pos, rot);

        // Update dictionary so new instance is tracked for future triggers
        originalTransforms[newObj] = (pos, rot);
    }

    private System.Collections.IEnumerator RespawnAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Respawn(obj);
    }
}