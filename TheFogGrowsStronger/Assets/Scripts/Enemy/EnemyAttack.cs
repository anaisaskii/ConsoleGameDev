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

    public float enemySpeed = 5f;

    public int enemyAttackDamage = 1;
    public int enemyAttackSpeed = 1;
    public float stoppingDistance = 1.5f;

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

        currentState = EnemyState.Chase;

        rb = GetComponent<Rigidbody>();

        enemybt = this.GetComponent<EnemyBT>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentState = EnemyState.Attack;
        }
    }

    private void FixedUpdate()
    { 
        HandleState();
    }

    // --Damage--

    //make public void to take the damage in individual classes
    //cause a virtual void can't be called i don't think..?
    protected virtual void Attack(int damage)
    {
        enemybt.progress();
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
    }

    protected virtual void ChasePlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;

        Debug.DrawRay(transform.position, direction * 2f, Color.red);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stoppingDistance)
        {
            Vector3 move = transform.position + direction * enemySpeed * Time.fixedDeltaTime;
            rb.MovePosition(move);

            // Optional: rotate to face the player
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, 10f * Time.fixedDeltaTime));
        }
    }

    protected virtual void AttackPlayer()
    {
        Attack(10);
    }

}