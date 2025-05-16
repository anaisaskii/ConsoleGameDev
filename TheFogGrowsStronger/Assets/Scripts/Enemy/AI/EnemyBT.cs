using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BTAI;

public class EnemyBT : MonoBehaviour
{
    private GameObject player;
    private Transform playerTransform;
    private PlayerHealth PLAYERHEALTH;

    private NavMeshAgent agent;

    private Animator animator;

    private float meleeRange = 3.0f; //range to melee attack
    private float rangedRange = 20.0f; //range to ranged attack
    private float combinedCooldown = 6f; // attack cooldowns
    private float lastAttackTime;

    private Root aiRoot;

    void Start()
    {
        //set up behaviour tree
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();

        aiRoot = BT.Root();

        var attackSelector = BT.Selector()
            .OpenBranch(
                BT.Sequence()
                    .OpenBranch(
                        BT.Condition(() => IsPlayerInMeleeRange()),
                        BT.Call(PerformLightMeleeAttack)
                    ),
                BT.Sequence()
                    .OpenBranch(
                        BT.Condition(() => IsPlayerInRangedRange() && !IsPlayerInMeleeRange()),
                        BT.Call(PerformRangedAttack)
                    )
            );

        aiRoot.OpenBranch(attackSelector);
    }

    // Runs the behaviour tree, this is called in the state machine every frame
    // when in attack state
    public void progress()
    {
        aiRoot.Tick();
    }

    //check if player is within range to perform a melee attack
    private bool IsPlayerInMeleeRange()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) <= meleeRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return isInRange && canAttack;
    }

    //check if player is within range to perform a ranged attack
    private bool IsPlayerInRangedRange()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) <= rangedRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return isInRange && canAttack;
    }

    //perform attacks
    private void PerformLightMeleeAttack()
    {
        animator.SetTrigger("AtkLight");
        lastAttackTime = Time.time;
    }

    private void PerformRangedAttack()
    {
        animator.SetTrigger("Shoot");
        lastAttackTime = Time.time;
    }

    public void playerTakeDamage(int damage)
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= meleeRange)
        {
            ApplyDamageToPlayer(damage);
        }
    }

    private void ApplyDamageToPlayer(int damage)
    {
        PLAYERHEALTH.TakeDamage(damage);
    }
}
