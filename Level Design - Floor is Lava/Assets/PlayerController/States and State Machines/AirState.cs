using UnityEngine;

public class AirState : PlayerState
{
    public override void Enter(StateMachine state_machine)
    {
        //Debug.Log("Air Enter");

        //Change color to blue.
        //player.rend.material.color = Color.blue;

        //Animations
        player.animator.SetBool("IsJumping", true);
    }

    public override void Exit(StateMachine state_machine)
    {
        //Debug.Log("Air Exit");
    }

    public override void StateUpdate(StateMachine state_machine)
    {
        //Debug.Log("Air Update");
    }

    public override void StateFixedUpdate(StateMachine state_machine)
    {
        //Debug.Log("Air FixedUpdate");

        //Acceleration
        if (player.direction != Vector2.zero) //Player is holding down a key.
            player.xz_velocity += new Vector2(player.wish_dir.x, player.wish_dir.z)
                                  * player.acceleration * Time.fixedDeltaTime;
        player.xz_velocity = Vector2.ClampMagnitude(player.xz_velocity, player.speed); //Only clamp the grounded movement. Do not affect y movement at all.

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
            if (player.direction == Vector2.zero)
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
            state_machine.TransitionToState(state_machine.states[0]);
        }

        #endregion
    }
}
