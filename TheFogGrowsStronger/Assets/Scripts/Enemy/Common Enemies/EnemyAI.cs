using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

//Base enemy AI/state machine class

public abstract class EnemyAI : MonoBehaviour
{
    //Enemy attributes
    public int enemyAttackDamage = 1;
    public int enemyAttackSpeed = 1;
    public float detectionRadius;
    public float enemySpeed = 5f;
    public TextMeshProUGUI cashText;

    // Player/Enemy Gameobjects
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Rigidbody rb;
    protected GameObject playerObj;
    protected Transform player;

    protected EnemySpawner enemySpawner;
    protected Transform[] waypoints;

    // Patrol logic (waypoints...)
    protected int currentWaypointIndex = 0;
    protected bool isWaiting = false;
    protected float waitTimer = 0f;
    protected float waitTime = 2f;

    // chase logic
    protected float chaseTimer = 0f;
    protected float maxChaseDuration = 10f;

    //Possible enemy states
    protected enum EnemyState { Patrol, Chase, Attack, Die }
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

    // Check for state changes each update
    protected virtual void FixedUpdate()
    {
        HandleState();
    }


    protected virtual void HandleState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is near enough to the enemy, attack
        if (distanceToPlayer <= detectionRadius && currentState != EnemyState.Attack)
        {
            currentState = EnemyState.Attack;
            return;
        }

        // call the appropriate function based on the state
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

    //override-able functions for each enemy
    protected abstract void Patrol();
    protected abstract void ChasePlayer();
    protected abstract void AttackPlayer();
}
