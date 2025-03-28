using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private NavMeshAgent agent;
    private Transform currentTarget;

    public TextMeshProUGUI cashText;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTarget = pointA; 
        agent.SetDestination(currentTarget.position);
    }

    //destroy object in final game but this is fine for now 
    public void EnemyDie()
    {
        GetComponent<MeshRenderer>().enabled = false;
        cashText.text = (int.Parse(cashText.text) + 10).ToString();
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
