using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerController : MonoBehaviour
{
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
        character_controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Accelerate
        velocity.x += acceleration * move_input.x;

        //Clamo the max speed.
        velocity.x = Mathf.Clamp(velocity.x, -move_speed, move_speed);

        //Apply friction.
        if (move_input == Vector2.zero)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, friction);
        }

        //Gravity
        velocity.y -= gravity;
        if (character_controller.isGrounded && velocity.y < -1f)
            velocity.y = -1f;


        //Move
        character_controller.Move(velocity * Time.fixedDeltaTime);
        
    }

    public Vector2 GetInput()
    {
        return move_input;
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
