using UnityEngine;

public class StateMachine : MonoBehaviour
{
    //Problems to solve:
    //3.) Give state a state machine varaible???
    //Not really a problem but could be something to do.

    //Current state the state machine is in.
    private State current_state;

    //Holds a list of all the possible states the state machine can have.
    public State[] states;

    void Start()
    {
        //Set the initial state.
        current_state = states[0];

        //Run enter for the initial state.
        current_state.Enter(this);
    }

    void Update()
    {
        current_state.StateUpdate(this);
    }

    void FixedUpdate()
    {
        current_state.StateFixedUpdate(this);
    }

    public void TransitionToState(State new_state)
    {
        //Exit the current state.
        current_state.Exit(this);

        //Change the current state.
        current_state = new_state;

        //Enter the new state.
        current_state.Enter(this);
    }
}
