using UnityEngine;

public class PlatformerCamera : MonoBehaviour
{
    public GameObject target;
    private PlatformerController controller;
    public Vector3 camera_offset = new Vector3(0, 0, -10);
    public float camera_speed = 5f;
    public float camera_padding = 2f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = target.GetComponent<PlatformerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Lerp the camera. Offset the final position of the camera so that it reacts to the player's inputs.
        Vector3 final_pos = target.transform.position + camera_offset + (new Vector3(controller.GetInput().x, 0, 0).normalized * camera_padding);
        transform.position = Vector3.Lerp(transform.position, final_pos, camera_speed * Time.fixedDeltaTime);
    }
}
