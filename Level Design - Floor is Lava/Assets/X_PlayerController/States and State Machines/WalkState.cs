using UnityEngine;

public class WalkState : PlayerState
{
    public override void Enter(StateMachine state_machine)
    {
        //Debug.Log("Walk Enter");

        //Change color to green.
        //player.rend.material.color = Color.green;

        //Aniamtions
        player.animator.SetBool("IsMoving", true);
    }

    public override void Exit(StateMachine state_machine)
    {
        //Debug.Log("Walk Exit");
    }

    public override void StateUpdate(StateMachine state_machine)
    {
        //Debug.Log("Walk Update");
    }

    public override void StateFixedUpdate(StateMachine state_machine)
    {
        //Debug.Log("Walk FixedUpdate");

        //Acceleration
        if (player.direction != Vector2.zero) //Player is holding down a key.
            player.xz_velocity += new Vector2(player.wish_dir.x, player.wish_dir.z) 
                                  * player.acceleration * Time.fixedDeltaTime;
        player.xz_velocity = Vector2.ClampMagnitude(player.xz_velocity, player.speed); //Only clamp the grounded movement. Do not affect y movement at all.

        

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
                state_machine.TransitionToState(state_machine.states[0]);
                return;
            }
        }

        //Air State
        //Only jump if space pressed and on the ground.
        if (player.jump && player.controller.isGrounded)
        {
            player.velocity = new Vector3(0, player.jump_impulse, 0);
            state_machine.TransitionToState(state_machine.states[2]);
        }

        //Enter air state if not on the ground.
        if (!player.controller.isGrounded)
        {
            state_machine.TransitionToState(state_machine.states[2]);
            return;
        }

        #endregion
    }
}
