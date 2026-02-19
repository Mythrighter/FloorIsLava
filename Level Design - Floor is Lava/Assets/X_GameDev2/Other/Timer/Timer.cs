using UnityEngine;
using UnityEngine.Events;

//A timer is something the keeps track of time.
//If a timer has >0 time it will begin to count down until no more time remains.
//Timers can be paused. 
    //A paused timer retains its current time value.
    //A timer can be paused/unpaused by using the SetPause() function.
    //A timer will be unpaused if StartTimer() is used.
public class Timer : MonoBehaviour
{
    //Event to signal when the timer has finished counting down.
    public UnityEvent TimerFinished;

    [Tooltip("The current time on the timer. If the timer has time, it will count down.")]
    [SerializeField] [Range(0f, 600f)] private float time; 
    private bool is_paused = false; //Flag that determines if the timer is paused.
    private bool has_finished = false; //Flag that determines if the timer has finished running.

    private void Start()
    {
        //Start the timer if the user set the time in the inspector.
        if (time > 0)
            StartTimer(time);
    }

    private void Update()
    {
        //Stop the timer from running if it is paused or has finished.
        if (is_paused || has_finished) 
            return;

        if (time > 0) //If the timer has time on it...
            time -= Time.deltaTime; //Countdown.
        else //Otherwise...
            StopTimer(); //Stop the timer.  

    }

    //Starts the timer by setting the time to t.
    //Acts as the setter for time.
    public void StartTimer(float t)
    {
        //Debug.Log("Timer started with " + time + " time.");
        time = t; //Assign the time passed in.
        is_paused = false;
        has_finished = false;
    }

    //Stops the timer by setting it to zero.
    public void StopTimer()
    {
        //Debug.Log("Timer has stopped");
        has_finished = true; //The timer has finished.
        time = 0; //So set its time to zero.
        TimerFinished.Invoke(); //Invoke the TimerFinished event.
    }

    //Gets the time from the timer.
    public float GetTime()
    {
        return time;
    }

    //Gets the finished state of the timer.
    public bool IsFinished()
    {
        return has_finished;
    }

    //Sets the paused status of the timer.
    public void SetPause(bool pause)
    {
        is_paused = pause;
    }

    //Gets the paused status of the timer.
    public bool GetPause()
    {
        return(is_paused);
    }
}
