using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BTAI;

public class BossBT : MonoBehaviour
{
    private GameObject player;
    private Transform playerTransform;
    private Health playerhealth;

    private Root aiRoot;

    private NavMeshAgent agent;

    private Animator animator;

    private float meleeRange = 20.0f;
    private float rangedRange = 30.0f;
    private float combinedCooldown = 6f;
    private float lastAttackTime;

    private float playerAggression = 0f;
    private float observationInterval = 5f;
    private float lastObservationTime;
    private int playerAttackCount;

    // Start is called before the first frame update
    void Start()
    {
        // Usual setup
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        playerhealth = player.GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        aiRoot = BT.Root();
        var selector = BT.Selector();

        var blockSequence = BT.Sequence()
            .OpenBranch(
                BT.Condition(ShouldBlock),
                BT.Call(PerformBlock)
            );

        var attackSequence = BT.Sequence()
            .OpenBranch(
                BT.Condition(CanPerformAttack),
                BT.Call(AdaptiveAttack)
            );

        selector.OpenBranch(blockSequence, attackSequence);
        aiRoot.OpenBranch(selector);
    }

    // Update is called once per frame
    public void Progress()
    {
        aiRoot.Tick();

        if (Time.time >= lastObservationTime + observationInterval)
        {
            EvaluatePlayerAggression();
        }
    }

    private void EvaluatePlayerAggression()
    {
        // Normalize attack count per interval, you can tune this.
        playerAggression = Mathf.Clamp01(playerAttackCount / 10f); // 0 = passive, 1 = aggressive
        playerAttackCount = 0;
        lastObservationTime = Time.time;

        Debug.Log("Player aggression score: " + playerAggression);
    }

    public void OnPlayerAttack()
    {
        playerAttackCount++;
    }

    private void AdaptiveAttack()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        Debug.Log(distance);

        if (distance <= meleeRange)
        {
            if (playerAggression < 0.5f)
            {
                PerformHeavyMeleeAttack();
            }
            else
            {
                PerformLightMeleeAttack();
            }
        }
        else if (distance <= rangedRange)
        {
            PerformRangedAttack();
        }

        lastAttackTime = Time.time;
    }

    private bool ShouldBlock()
    {
        return playerAggression >= 0.7f && Random.value > 0.5f;
    }

    private void PerformBlock()
    {
        animator.SetTrigger("Dodge");
        Debug.Log("Boss is dodging!");
    }

    private bool CanPerformAttack()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) <= meleeRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return canAttack && isInRange;
    }

    private void PerformLightMeleeAttack()
    {
        agent.isStopped = true;
        agent.ResetPath();

        animator.SetTrigger("Swipe");
        Debug.Log("Boss Performed light melee attack!");
        lastAttackTime = Time.time;

        agent.isStopped = false;
    }

    private void PerformHeavyMeleeAttack()
    {
        agent.isStopped = true;
        agent.ResetPath();

        animator.SetTrigger("Kick");
        Debug.Log("Boss Performed heavy melee attack!");
        lastAttackTime = Time.time;

        agent.isStopped = false;
    }

    private void PerformRangedAttack()
    {
        Debug.Log("Boss Performed shoot attack!");
        //animator.SetTrigger("Shoot");
        //shoot
    }

}
