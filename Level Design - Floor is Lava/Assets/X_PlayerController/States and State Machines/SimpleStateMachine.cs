using UnityEngine;

public class SimpleStateMachine : MonoBehaviour
{
    //Components
    private Player player;
    
    //Player States
    enum States
    {
        Idle,
        Walk,
        Air,
    }
    private States current_state = States.Idle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get Components
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log("State: " + current_state.ToString());

        switch (current_state)
        {
            case States.Idle:
                //Debug.Log("0");
                IdleState();
                break;

            case States.Walk:
                //Debug.Log("1");
                WalkState();
                break;

            case States.Air:
                //Debug.Log("2");
                AirState();
                break;
        }
    }

    private void IdleState()
    {
        //Debug.Log("Idle State");

        //Change color to dark green.
        //player.rend.material.color = new Color(0f, 0.3921569f, 0f, 1f);

        #region Change States
        //Walk State
        if (player.direction != Vector2.zero)
        {
            current_state = States.Walk;
            return;
        }

        //Air State
        if (player.jump)
        {
            player.velocity = new Vector3(0, player.jump_impulse, 0);
            current_state = States.Air;
            return;
        }
        #endregion
    }

    private void WalkState()
    {
        //Change color to green.
        //player.rend.material.color = Color.green;

        //Acceleration
        player.xz_velocity += player.direction * player.acceleration * Time.fixedDeltaTime;
        player.xz_velocity = Vector2.ClampMagnitude(player.xz_velocity, player.speed); //Only clamp the grounded movement. Do not affeect y movement at all.

        //Friction
        if (player.direction == Vector2.zero)
            player.xz_velocity = Vector2.MoveTowards(player.xz_velocity, 
                                                     Vector2.zero, 
                                                     player.friction * Time.fixedDeltaTime);

        //Construct Velocity - Create a new velocity using the xz_velocity.
        player.velocity = new Vector3(player.xz_velocity.x, -player.gravity, player.xz_velocity.y);

        //Move
        player.controller.Move(player.velocity * Time.fixedDeltaTime);

        //Debug.Log(player.direction);

        #region Change States

        //Idle State
        if (player.controller.velocity == Vector3.zero) //If the player has stopped moving.
        {
            //Stop any residual velocity.
            player.xz_velocity = Vector2.zero;

            if (player.direction == Vector2.zero) //Only if the player lets go of the WASD keys.
            {
                //Should we set the velocity and change the state.
                player.velocity = new Vector3(0, player.gravity, 0);
                current_state = States.Idle;
                return;
            }
        }

        //Air State
        //Only jump if space pressed and on the ground.
        if (player.jump && player.controller.isGrounded)
        {
            player.velocity = new Vector3(0, player.jump_impulse, 0);
            current_state = States.Air;
        }

        //Enter air state if not on the ground.
        if(!player.controller.isGrounded)
        {
            current_state = States.Air;
            return;
        }

        #endregion
    }

    private void AirState()
    {
        //Change color to blue.
        //player.rend.material.color = Color.blue;

        //Acceleration
        player.xz_velocity += player.direction * player.acceleration * Time.fixedDeltaTime;
        player.xz_velocity = Vector2.ClampMagnitude(player.xz_velocity, player.speed); //Only clamp the grounded movement. Do not affeect y movement at all.

        //Friction
        //if (player.direction == Vector2.zero)
        //    player.xz_velocity = Vector2.MoveTowards(player.xz_velocity,
        //                                             Vector2.zero,
        //                                             player.friction * Time.fixedDeltaTime);

        //Gravity
        player.velocity.y -= player.gravity * Time.fixedDeltaTime;

        //Construct Velocity - Create a new velocity using the xz_velocity.
        player.velocity = new Vector3(player.xz_velocity.x, player.velocity.y, player.xz_velocity.y);

        //Move
        player.controller.Move(player.velocity * Time.fixedDeltaTime);

        //Debug.Log(player.direction);

        #region Change States

        //Idle State
        if (player.controller.isGrounded)
        {
            if(player.direction == Vector2.zero)
            {
                //Cancel any movement since the player is not holding a key.
                player.velocity = new Vector3(0, player.gravity, 0);
                player.xz_velocity = Vector2.zero;
            } 
            else 
            {
                //Otherwise just set player y veloicty to gravity and keep any horizontal velocity.
                player.velocity.y = player.gravity;
            }    
            current_state = States.Idle;
        }

        #endregion
    }
}
