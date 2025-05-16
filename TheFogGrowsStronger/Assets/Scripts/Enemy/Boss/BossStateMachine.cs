using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossStateMachine : EnemyAI
{
    //Used for both bosses
    private BossBT bossBT;
    private Boss2BT boss2bt;

    protected override void Start()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            transform.position = hit.position; // Snap to nearest NavMesh
        }

        //Run all default starting code
        base.Start();

        // Set correct waypints based on boss
        if (this.gameObject.name == "Boss_1")
        {
            bossBT = GetComponent<BossBT>();
            waypoints = enemySpawner.bossWaypoints;
        }
        else
        {
            boss2bt = GetComponent<Boss2BT>();
            waypoints = enemySpawner.boss2Waypoints;
        }
        
    }

    // Movement animations are based on the enemy's current movement speed
    private void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    protected override void AttackPlayer()
    {
        Attack(enemyAttackDamage);
    }

    // Attack State
    // The boss's behaviour tree will handle attacks
    private void Attack(int damage)
    {
        if(this.gameObject.name == "Boss_1")
        {
            bossBT.Progress();
        }
        else
        {
            boss2bt.Progress();
        }
        
        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.position) >= detectionRadius)
        {
            currentState = EnemyState.Chase;
            agent.isStopped = false;
        }
    }

    // Patrol State
    // Boss will move between waypoints, waiting at each one for a few seconds
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
    // If the player is near (not too near - attack state) the boss will chase them
    // if the boss is chasing for 10 seconds it will enter the patrol state
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
