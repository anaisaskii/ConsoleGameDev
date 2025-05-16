using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

// Common enemy AI/state machine
public enum EnemyType
{
    ENEMY1,
    ENEMY2,
    BOSS
}

public class CommonEnemyAI : EnemyAI
{
    private EnemyBT enemybt;

    protected override void Start()
    {
        base.Start();
        enemybt = GetComponent<EnemyBT>();
        waypoints = enemySpawner.waypoints;
    }

    protected override void AttackPlayer()
    {
        Attack(enemyAttackDamage);
    }

    // --Damage--
    private void Attack(int damage)
    {
        enemybt.progress();

        NavMeshPath path = new NavMeshPath();
        Vector3 targetPos = player.position;

        bool hasPath = agent.CalculatePath(targetPos, path)
                       && path.status == NavMeshPathStatus.PathComplete;

        if (hasPath)
        {
            agent.SetDestination(targetPos);
        }
        else
        {
            agent.ResetPath();
        }

        if (Vector3.Distance(transform.position, targetPos) >= detectionRadius)
        {
            currentState = EnemyState.Chase;
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
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
            agent.isStopped = true;
            agent.ResetPath();
            currentState = EnemyState.Attack;
            chaseTimer = 0f;
        }
    }

}