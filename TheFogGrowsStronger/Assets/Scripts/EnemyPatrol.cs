using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private NavMeshAgent agent;
    private Transform currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTarget = pointA; 
        agent.SetDestination(currentTarget.position);
    }

    void Update()
    {
        // cgheck if agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Swap to the other point
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
            agent.SetDestination(currentTarget.position);
        }
    }
}
