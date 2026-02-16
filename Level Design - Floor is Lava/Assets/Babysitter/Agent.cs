using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public GameObject target;
    private NavMeshAgent nav_agent;

    public void Start()
    {
        //Get Components.
        nav_agent = GetComponent<NavMeshAgent>();

        //Set the destination for the agent.
        nav_agent.SetDestination(target.transform.position);
    }
}
