using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

//Base enemy AI/state machine class

public abstract class EnemyAI : MonoBehaviour
{
    public int enemyAttackDamage = 1;
    public int enemyAttackSpeed = 1;
    public float detectionRadius;
    public float enemySpeed = 5f;
    public TextMeshProUGUI cashText;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected Rigidbody rb;
    protected GameObject playerObj;
    protected Transform player;

    protected EnemySpawner enemySpawner;
    protected Transform[] waypoints;

    protected int currentWaypointIndex = 0;
    protected bool isWaiting = false;
    protected float waitTimer = 0f;
    protected float waitTime = 2f;

    protected float chaseTimer = 0f;
    protected float maxChaseDuration = 10f;

    protected enum EnemyState { Idle, Patrol, Chase, Attack, Die }
    protected EnemyState currentState;

    protected virtual void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();

        currentState = EnemyState.Patrol;
    }

    protected virtual void FixedUpdate()
    {
        HandleState();
    }

    protected virtual void HandleState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius && currentState != EnemyState.Attack)
        {
            currentState = EnemyState.Attack;
            return;
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
        }
    }

    protected abstract void Patrol();
    protected abstract void ChasePlayer();
    protected abstract void AttackPlayer();
}
