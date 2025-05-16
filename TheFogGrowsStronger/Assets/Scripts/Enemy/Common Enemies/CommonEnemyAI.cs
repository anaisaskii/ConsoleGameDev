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

    // Attack State
    // The enemy's behaviour tree will handle attacks
    private void Attack(int damage)
    {
        enemybt.progress(); // Run the behaviour tree

        NavMeshPath path = new NavMeshPath();
        Vector3 targetPos = player.position;

        //check if there is a valid path
        bool hasPath = agent.CalculatePath(targetPos, path) && path.status == NavMeshPathStatus.PathComplete;

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

    // Patrol State
    // Enemy will move between waypoints, waiting at each one for a few seconds
    // Once it has visited all waypoints, it will start again
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

    // Chase State
    // If the player is near (not too near - attack state) the enemy will chase them
    // if the enemy is chasing for 10 seconds it will enter the patrol state
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
            animator.SetBool("Move", true); // set movement animation
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