using UnityEngine;

//The idle state of the player.
public class IdleState : PlayerState
{
    public override void Enter(StateMachine state_machine)
    {
        //Debug.Log("Idle Enter");

        //Change color to dark green.
        //player.rend.material.color = new Color(0f, 0.3921569f, 0f, 1f);

        //Animations
        player.animator.SetBool("IsMoving", false);
        player.animator.SetBool("IsJumping", false);
    }

    public override void Exit(StateMachine state_machine)
    {
        //Debug.Log("Idle Exit");
    }

    public override void StateUpdate(StateMachine state_machine)
    {
        //Debug.Log("Idle Update");
    }

    public override void StateFixedUpdate(StateMachine state_machine)
    {
        //Debug.Log("Idle FixedUpdate");

        #region Change States

        //Walk State
        if (player.direction != Vector2.zero)
        {
            state_machine.TransitionToState(state_machine.states[1]);
            return;
        }

        //Air State
        if (player.jump)
        {
            player.velocity = new Vector3(0, player.jump_impulse, 0);
            state_machine.TransitionToState(state_machine.states[2]);
            return;
        }

        #endregion
    }
}
