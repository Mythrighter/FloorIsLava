using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public GameObject[] patrolPoints;
    private NavMeshAgent nav_agent;

    public float waitTime;

    public void Start()
    {
        //Get components.
        nav_agent = GetComponent<NavMeshAgent>();
        

    }

    public void Update()
    {
        waitTime -= Time.deltaTime;
        
        if(waitTime <= 0)
        {
            NewPatrol();
        }

    }

    public void NewPatrol()
    {
        //Generate random number as random patrol point.
        int randomNumber = Random.Range(0, patrolPoints.Length);

        //Identify the point as an object
        GameObject randomObject = patrolPoints[randomNumber];


        //send agent to that random point.
        nav_agent.SetDestination(randomObject.transform.position);

        waitTime = Random.Range(10, 30);


    }
}
