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

    private float meleeRange = 5.0f;
    private float combinedCooldown = 6f;
    private float lastAttackTime;

    private Root aiRoot;

    private bool isJumping;
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
                BT.Condition(CanPerformAttack),
                BT.Call(PerformRangedAttack)
            );

        selector.OpenBranch(meleeSequence, rangedSequence);
        aiRoot.OpenBranch(selector);
    }

    void Update()
    {
        if (isJumping)
        {
            float timeElapsed = Time.time - startTime;
            float t = timeElapsed / jumpDuration;

            if (t < 1f)
            {
                transform.position = Vector3.Lerp(jumpStartPosition, jumpTargetPosition, t);
            }
            else
            {
                isJumping = false;
                agent.enabled = true; // Re-enable agent after jump
            }
        }
    }

    public void progress()
    {
        if (!isJumping) // Don't tick BT during a jump
            aiRoot.Tick();
    }

    private bool IsPlayerInMeleeRange()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) <= meleeRange;
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
        Debug.Log("Performed light melee attack!");
        lastAttackTime = Time.time;
        // animator.SetTrigger("meleeLight");
    }

    private void PerformHeavyMeleeAttack()
    {
        Debug.Log("Performed heavy melee attack!");
        lastAttackTime = Time.time;

        // Move quickly toward the player
        if (agent.enabled)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    private void PerformRangedAttack()
    {
        Debug.Log("Performed ranged jump attack!");
        lastAttackTime = Time.time;

        jumpStartPosition = transform.position;
        jumpTargetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);

        isJumping = true;
        startTime = Time.time;

        // Temporarily disable NavMeshAgent during jump
        agent.enabled = false;
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
