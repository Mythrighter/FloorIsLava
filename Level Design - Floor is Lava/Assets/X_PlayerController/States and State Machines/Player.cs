using UnityEngine;

using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Components
    [HideInInspector] public CharacterController controller;

    //[HideInInspector] public MeshRenderer rend;

    public Camera cam;

    public Animator animator;

    //Movement
    public float speed = 5;
    [HideInInspector] public Vector2 direction = Vector2.zero; //This should probably be called input.
    [HideInInspector] public Vector3 velocity = Vector3.zero;
    [HideInInspector] public Vector2 xz_velocity = Vector2.zero; //Vector representing the grounded movement of the player.
    [HideInInspector] public Vector3 wish_dir = Vector2.zero;

    //Accelerations
    public float acceleration = 30f;
    public float friction = 25f;
    public float gravity = 9.8f;

    //Forces
    public float jump_impulse = 10;

    //Jumping
    [HideInInspector] public bool jump = false;

    //Input
    //public InputAction move_action;
    //public InputAction jump_action;

    void Start()
    {
        //Hide Mouse Cursor
        Cursor.lockState = CursorLockMode.Locked;

        //Get Components
        controller = GetComponent<CharacterController>();

        //rend = GetComponent<MeshRenderer>();

        //Find the references to the "Move" and "Jump" actions.
        //move_action = InputSystem.actions.FindAction("Move");
        //jump_action = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        ////Left/Right
        //direction.x = Input.GetAxisRaw("Horizontal");

        ////Forward/Back
        //direction.y = Input.GetAxisRaw("Vertical");

        ////Normalize the direction.
        //direction = Vector3.Normalize(direction);

        //Read values directly to input.
        //direction = move_action.ReadValue<Vector2>();

        //Find the angle we need for our new movement.
        float target_angle = Mathf.Atan2(direction.x, direction.y) *
                             Mathf.Rad2Deg + cam.transform.eulerAngles.y;

        //Use the target_angle to find the direction to move in.
        wish_dir = Quaternion.Euler(0f, target_angle, 0f) * Vector3.forward;
        wish_dir = wish_dir.normalized;

        //Record if the jump button is being pressed.
        //jump = Input.GetButton("Jump");
        //jump = jump_action.IsPressed();

        //Player msut press a key in order to get the player to turn.
        if (direction != Vector2.zero)
        {
            //Make player turn in the direction of wish_dir.
            transform.forward = Vector3.Lerp(transform.forward, wish_dir, 8f * Time.deltaTime);
        }
    }

    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            jump = true;
        else
            jump = false;
        
        //Debug.Log(context.ReadValueAsButton());
    }

    private void OnDrawGizmos()
    {
        #region Player's Gizmos

        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, transform.position + direction * 2);

        #endregion  
    }
}
