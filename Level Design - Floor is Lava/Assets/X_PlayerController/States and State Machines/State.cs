using UnityEngine;

//This is the base state script.
//All other scripts will inherit from this script.
public class State : MonoBehaviour
{
    //The state machine calls this when a state is transitioned into. 
    public virtual void Enter(StateMachine state_machine)
    {

    }

    //The state machine calls this when a state is transitioned out of.
    public virtual void Exit(StateMachine state_machine)
    {

    }

    //The state machine calls this method during its Update() function.
    public virtual void StateUpdate(StateMachine state_machine)
    {

    }

    //The state machine calls this method during its FixedUpdate() function.
    public virtual void StateFixedUpdate(StateMachine state_machine)
    {

    }
}
