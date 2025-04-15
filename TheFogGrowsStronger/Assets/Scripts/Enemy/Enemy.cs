using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using TMPro;
public enum EnemyType
{
    ENEMY1,
    ENEMY2,
    BOSS
}

public class Enemy : MonoBehaviour
{
    //set what type of enemy this is
    public EnemyType enemyType;

    public int enemyHealth = 100;
    public int enemySpeed = 1;
    public int enemyAttackDamage = 1;
    public int enemyAttackSpeed = 1;

    public float attackCooldownTime;
    private float attackCooldown;

    public Transform player;

    //states and stuff
    private enum EnemyState { Idle, Patrol, Chase, Attack, Die };
    private EnemyState currentState;

    public TextMeshProUGUI cashText;

    // --Damage--

    //make public void to take the damage in individual classes
    //cause a virtual void can't be called i don't think..?
    protected virtual void Attack(int damage)
    {

    }

    protected virtual void ApplyDamage()
    {
        enemyHealth -= enemyAttackDamage;
    }

    public void EnemyDie()
    {
        Die();
    }

    protected virtual void Die()
    {
        GetComponent<MeshRenderer>().enabled = false;
        cashText.text = (int.Parse(cashText.text) + 10).ToString();
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