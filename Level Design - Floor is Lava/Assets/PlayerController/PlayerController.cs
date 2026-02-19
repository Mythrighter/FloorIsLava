using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Camera
    public Camera cam;
    private Vector2 look_input = Vector2.zero;
    private float look_speed = 60;
    private float hor_look_angle = 0f;
    public bool invert_x = false;
    public bool invert_y = false;
    private int invert_factor_x = 1;
    private int invert_factor_y = 1;
    [Range(0.01f , 2f)] public float sensitivity = 1;

    //Movement
    public float move_speed = 10f;
    public float acceleration = 0.2f;
    public float friction = 0.2f;
    public float gravity = 0.2f;
    public float jump_power = 20f;
    private Vector2 move_input = Vector2.zero;
    private CharacterController character_controller;
    private Vector3 velocity = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Hide cursor on game start.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        character_controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Move();
    }

    private void Look()
    {
        //Invert Camera
        invert_factor_x = invert_x ? -1 : 1;
        invert_factor_y = invert_y ? -1 : 1;

        //Rotate the player to look left/right.
        transform.Rotate(Vector2.up, look_input.x * invert_factor_x * look_speed * sensitivity * Time.deltaTime);

        //Rotate the camera to look up/down.
        float angle = look_input.y * invert_factor_y * look_speed * sensitivity * Time.deltaTime;
        hor_look_angle -= angle;
        hor_look_angle = Mathf.Clamp(hor_look_angle, -90, 90);
        cam.transform.localRotation = Quaternion.Euler(hor_look_angle, 0, 0);
    }

    private void Move()
    {
        //Accelerate
        velocity += acceleration * (transform.right * move_input.x + transform.forward * move_input.y);

        //Clamp the max speed.
        Vector2 xz_velocity = new Vector2(velocity.x, velocity.z);
        xz_velocity = Vector2.ClampMagnitude(xz_velocity, move_speed);

        //Apply friction.
        if(move_input == Vector2.zero)
        {
            xz_velocity = Vector2.MoveTowards(xz_velocity, Vector2.zero, friction);
        }

        //Gravity
        velocity.y -= gravity;
        if (character_controller.isGrounded && velocity.y < -1f)
            velocity.y = -1f;

        //Reconstruct Velocity
        velocity = new Vector3(xz_velocity.x, velocity.y, xz_velocity.y);
        

        //Move
        character_controller.Move(velocity * Time.deltaTime);
    }

    public void GetLookInput(InputAction.CallbackContext context)
    {
        look_input = context.ReadValue<Vector2>();
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        move_input = context.ReadValue<Vector2>();
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (character_controller.isGrounded)
            {
                velocity.y = jump_power;
            }
        }
    }

}
