using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class StateMachineBase : MonoBehaviour
{
    public AgentState currentState = AgentState.Idle;

    public float scanInterval = 1f;
    public float interactionDistance = 1.5f;

    private NavMeshAgent agent;
    private Item currentTarget;

    private VisionCone vision;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<VisionCone>();

        StartCoroutine(StateLoop());
    }

    IEnumerator StateLoop()
    {
        while (true)
        {
            switch (currentState)
            {
                case AgentState.Idle:
                    currentState = AgentState.Searching;
                    break;

                case AgentState.Searching:
                    currentTarget = vision.FindVisibleTarget();

                    if (currentTarget != null)
                    {
                        agent.SetDestination(currentTarget.transform.position);
                        currentState = AgentState.MovingToTarget;
                    }
                    break;

                case AgentState.MovingToTarget:
                    if (currentTarget == null)
                    {
                        currentState = AgentState.Searching;
                        break;
                    }

                    float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

                    if (dist <= interactionDistance)
                    {
                        agent.ResetPath();
                        currentState = AgentState.Interacting;
                    }
                    break;

                case AgentState.Interacting:
                    if (currentTarget != null)
                    {
                        Interact(currentTarget);
                    }

                    currentTarget = null;
                    currentState = AgentState.Searching;
                    break;                        
            }

            yield return new WaitForSeconds(scanInterval);
        }
    }

    void Interact(Item item)
    {
        //automatically moves it back to proper place
        item.Restore();


    }

    
}
