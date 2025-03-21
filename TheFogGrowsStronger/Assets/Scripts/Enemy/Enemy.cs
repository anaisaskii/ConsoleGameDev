using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    public int enemySpeed = 2;
    public int attackDamage = 2;

    public float attackCooldownTime;
    private float attackCooldown;

    public Transform player;

    //states and stuff
    private enum EnemyState { Idle, Patrol, Chase, Attack, Die };
    private EnemyState currentState;



    // --Damage--

    //make public void to take the damage in individual classes
    //cause a virtual void can't be called i don't think..?
    protected virtual void Attack(int damage)
    {

    }

    protected virtual void ApplyDamage()
    {
        health -= attackDamage;
    }

    protected virtual void Die()
    {

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
            case EnemyState.Die:
                Die();
                break;
        }
    }

    protected virtual void Patrol()
    {
        // Patrol logic (shared for all enemies or can be overridden)
    }

    protected virtual void ChasePlayer()
    {
        // Chase player logic
    }

    protected virtual void AttackPlayer()
    {
        if (attackCooldown <= 0)
        {
            // Attack logic
            attackCooldown = attackCooldownTime;
        }
    }

}