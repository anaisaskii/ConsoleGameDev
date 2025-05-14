using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BTAI;

public class EnemyBT : MonoBehaviour
{
    private GameObject player;
    private Transform playerTransform;
    private Health playerhealth;

    private NavMeshAgent agent;

    private Animator animator;

    private float meleeRange = 3.0f;
    private float rangedRange = 20.0f;
    private float combinedCooldown = 6f;
    private float lastAttackTime;

    private Root aiRoot;

    private float jumpDuration = 1f;
    private float startTime;
    private Vector3 jumpStartPosition;
    private Vector3 jumpTargetPosition;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        playerhealth = player.GetComponent<Health>();
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

    void Update()
    {
        
    }

    public void progress()
    {
        aiRoot.Tick();
        
    }

    private bool IsPlayerInMeleeRange()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) <= meleeRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return isInRange && canAttack;
    }

    private bool IsPlayerInRangedRange()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) <= rangedRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return isInRange && canAttack;
    }


    private void PerformLightMeleeAttack()
    {
        animator.SetTrigger("AtkLight");
        Debug.Log("Performed light melee attack!");
        lastAttackTime = Time.time;
    }

    private void PerformRangedAttack()
    {
        animator.SetTrigger("Shoot");
        //shoot
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
        playerhealth.TakeDamage(damage);
    }
}
