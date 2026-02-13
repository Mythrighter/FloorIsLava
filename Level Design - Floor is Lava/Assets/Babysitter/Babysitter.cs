using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Babysitter : MonoBehaviour
{
    [SerializeField] float waitTimeOnWayPoint = 1f;
    [SerializeField] Path path;

    NavMeshAgent agent;
    //Animator animator;

    float time = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
    }

    private void Start()
    {
        agent.destination = path.GetCurrentWayPoint();
    }

    private void Update()
    {
        if (agent.remainingDistance <= 0.1f)
        {
            time += Time.deltaTime;
            if (time >= waitTimeOnWayPoint)
            {
                time = 0f;
                agent.destination = path.GetNextWayPoint();
            }
        }

        float noramlizedSpeed = Mathf.InverseLerp(0f, agent.speed, agent.velocity.magnitude);
        //animator.SetFloat("speed", noramlizedSpeed);

    }


}
