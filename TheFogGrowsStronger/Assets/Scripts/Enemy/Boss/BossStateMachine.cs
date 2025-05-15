using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossStateMachine : EnemyAI
{
    private BossBT bossBT;

    //set enemy type to determine waypoints

    protected override void Start()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            transform.position = hit.position; // Snap to nearest NavMesh
        }
        else
        {
            Debug.LogError($"{gameObject.name} could not find NavMesh nearby!");
        }

        base.Start();
        bossBT = GetComponent<BossBT>();
        waypoints = enemySpawner.bossWaypoints;
    }

    private void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    protected override void AttackPlayer()
    {
        Attack(enemyAttackDamage);
    }

    // --Damage--
    private void Attack(int damage)
    {
        bossBT.Progress();
        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.position) >= detectionRadius)
        {
            currentState = EnemyState.Chase;
            agent.isStopped = false;
        }
    }

    protected override void Patrol()
    {
        if (waypoints.Length == 0) return;

        

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
    }


    protected override void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        chaseTimer += Time.deltaTime;

        if (chaseTimer >= maxChaseDuration)
        {
            currentState = EnemyState.Patrol;
            chaseTimer = 0f;
            return;
        }

        if (distance > detectionRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
            currentState = EnemyState.Attack;
            chaseTimer = 0f;
        }
    }
}
