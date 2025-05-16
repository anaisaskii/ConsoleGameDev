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

    private NavMeshAgent agent; //AI agent for navmesh

    private Animator animator;

    private float meleeRange = 20.0f; //range to melee attack
    private float rangedRange = 30.0f; //range to ranged attack
    private float combinedCooldown = 6f; // attack cooldowns
    private float lastAttackTime;

    private float playerAggression = 0f; //how agressive the player is
    private float observationInterval = 5f; //how often to check agression
    private float lastObservationTime;
    private int playerAttackCount;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        playerhealth = player.GetComponent<Health>();

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        //set up behaviour tree

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

    // Runs the behaviour tree, this is called in the state machine every frame
    // when in attack state
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
        // normalize attack count per interval
        //if 0, not agressive
        //if 1, agressive
        playerAggression = Mathf.Clamp01(playerAttackCount / 10f);
        playerAttackCount = 0;
        lastObservationTime = Time.time;

        Debug.Log("Player aggression score: " + playerAggression);
    }

    //increase player aggression on attack
    public void OnPlayerAttack()
    {
        playerAttackCount++;
    }

    //Check if player is near to enemy to decide attack
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

    // animation dodges instead of blocking
    // if the player agression is high
    // also randomly chooses beteween dodging and light attack
    private bool ShouldBlock()
    {
        return playerAggression >= 0.7f && Random.value > 0.5f;
    }

    private void PerformBlock()
    {
        animator.SetTrigger("Dodge");
    }

    private bool CanPerformAttack()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) <= meleeRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return canAttack && isInRange;
    }

    //perform attacks
    //ideally this should also affect the player health :/
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
        //Did not get time to implement the ranged attack :(
    }

}
