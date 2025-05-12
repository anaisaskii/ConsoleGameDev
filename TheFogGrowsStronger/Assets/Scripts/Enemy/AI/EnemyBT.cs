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

    private float meleeRange = 5.0f;
    private float rangedRange = 15.0f;
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

        var selector = BT.Selector();

        var meleeAttackSelector = BT.Selector()
            .OpenBranch(
                BT.Sequence()
                    .OpenBranch(
                        BT.Condition(() =>
                        {
                            int attackType = Random.Range(0, 2);
                            return attackType == 0;
                        }),
                        BT.Call(PerformLightMeleeAttack)
                    ),
                BT.Sequence()
                    .OpenBranch(
                        BT.Condition(() => true),
                        BT.Call(PerformHeavyMeleeAttack)
                    )
            );

        var meleeSequence = BT.Sequence()
            .OpenBranch(
                BT.Condition(IsPlayerInMeleeRange),
                meleeAttackSelector
            );

        var rangedSequence = BT.Sequence()
            .OpenBranch(
                BT.Condition(IsPlayerInRangedRange),
                BT.Call(PerformRangedAttack)
            );

        selector.OpenBranch(meleeSequence, rangedSequence);
        aiRoot.OpenBranch(selector);
    }

    void Update()
    {
        
    }

    public void progress()
    {
        aiRoot.Tick();
        float distance = Vector3.Distance(agent.destination, playerTransform.position);
        if (distance > 0.5f)
        {
            agent.SetDestination(playerTransform.position);
        }
        else
        {

        }
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

    private bool CanPerformAttack()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) >= meleeRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return canAttack && isInRange;
    }

    private void PerformLightMeleeAttack()
    {
        animator.SetTrigger("AtkLight");
        Debug.Log("Performed light melee attack!");
        lastAttackTime = Time.time;
        // animator.SetTrigger("meleeLight");
    }

    private void PerformHeavyMeleeAttack()
    {
        Debug.Log("Performed heavy melee attack!");
        lastAttackTime = Time.time;

        playerhealth.TakeDamage(10);
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
