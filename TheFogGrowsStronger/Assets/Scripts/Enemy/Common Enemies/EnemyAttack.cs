using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

// This handles the enemy attacks and attack states
// We can change using a behaviour tree and state machine (from AI module...)

//can choose enemy type to quickly set attack damage and speed by default
public enum EnemyType
{
    ENEMY1,
    ENEMY2,
    BOSS
}

public class EnemyAttack : MonoBehaviour
{
    //set what type of enemy this is
    public EnemyType enemyType;

    public int enemyAttackDamage = 1;
    public int enemyAttackSpeed = 1;

    public float stoppingDistance;
    public float enemySpeed = 5f;

    private Animator animator;

    private NavMeshAgent agent;
    private Rigidbody rb;

    private GameObject playerObj;
    private Transform player;

    //states and stuff
    private enum EnemyState { Idle, Patrol, Chase, Attack, Die };
    private EnemyState currentState;

    public TextMeshProUGUI cashText;
    EnemyBT enemybt;

    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;
        animator = this.GetComponent<Animator>();

        currentState = EnemyState.Chase;

        rb = GetComponent<Rigidbody>();

        agent = this.GetComponent<NavMeshAgent>();

        enemybt = this.GetComponent<EnemyBT>();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    { 
        HandleState();
    }

    // --Damage--
    protected virtual void Attack(int damage)
    {
        enemybt.progress();
        agent.SetDestination(player.transform.position);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance >= stoppingDistance)
        {
            Debug.Log("Entering Chase state!");
            currentState = EnemyState.Chase;
        }
    }

    // --Movement--

    //change states if they're supposed to
    protected virtual void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
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

    protected virtual void Patrol()
    {
        // Patrol logic (shared for all enemies or can be overridden)
        // if player gets out of chase radius
        // walk to each waypoint
    }

    protected virtual void ChasePlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // If we're far enough, keep chasing
        if (distance > stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("Move", true);
        }
        else if (distance <= stoppingDistance)
        {
            Debug.Log("Entering attack state!");
            animator.SetBool("Move", false);
            agent.isStopped = true;
            agent.ResetPath();

            // Optionally switch state
            currentState = EnemyState.Attack;
        }
    }

    protected virtual void AttackPlayer()
    {
        Attack(10);
    }

}