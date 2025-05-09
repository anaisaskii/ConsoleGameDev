using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

/// <summary>
/// Based on AI module behaviour tree tutorial from Moodle
/// Adapts based on user playstyle
/// </summary>

public class EnemyBT : MonoBehaviour
{
    private Transform player;
    //public PlayerHealth playerhealth;

    private float meleeRange = 5.0f; // how near the player has to be to attack
    private float combinedCooldown = 6f; // a joint cooldown so attacks can't be spammed
    private float lastAttackTime; // check when the enemy last attacked

    private Root aiRoot; // root for behaviour tree

    private Animator animator;

    private bool isJumping; // check if jumping
    private float jumpDuration = 1f; // how long does the enemy take to jump
    private float startTime; //start time of jump

    private Vector3 jumpStartPosition;
    private Vector3 jumpTargetPosition;

    public void Start()
    {
        //find player gameobject
        this.player = GameObject.Find("Target").transform;

        //get player health from player
        //playerhealth.GetComponent<PlayerHealth>();

        animator = this.GetComponent<Animator>();
        animator.speed = 0.9f; // 1 was too fast...

        aiRoot = BT.Root();

        // create selector node
        var selector = BT.Selector();

        // select which melee attack to use
        // chooses a random number (0, 1) and if 0, do a light attack
        var meleeAttackSelector = BT.Selector()
            .OpenBranch(
                BT.Sequence()
                    .OpenBranch(
                        BT.Condition(() =>
                        {
                            int attackType = Random.Range(0, 2);
                            return attackType == 0; // light melee condition
                        }),
                        BT.Call(PerformLightMeleeAttack)
                    ),
                BT.Sequence()
                    .OpenBranch(
                        BT.Condition(() => true), // heavy melee condition
                        BT.Call(PerformHeavyMeleeAttack)
                    )
            );

        // will check if the player is near enough to attack
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

        Debug.Log("BTAttack initialized.");
    }

    private void Update()
    {
        if (isJumping)
        {
            float timeElapsed = Time.time - startTime;
            if (timeElapsed < jumpDuration)
            {
                // calculate the interpolation factor
                float t = timeElapsed / jumpDuration;

                // Move the enemy towards the target position
                transform.position = Vector3.Lerp(jumpStartPosition, jumpTargetPosition, t);
            }
            else
            {
                // End the jump
                isJumping = false;
            }
        }
    }

    public void progress()
    {
        aiRoot.Tick();

        // calculate the direction to the player
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;  // Ignore vertical rotation

        // rotate extra 90 degrees (so it faces forward)
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        targetRotation *= Quaternion.Euler(0f, 90f, 0f); // Rotate an additional 90 degrees on the Y-axis
        transform.rotation = targetRotation;
    }

    private bool IsPlayerInMeleeRange()
    {
        bool isInRange = Vector3.Distance(transform.position, player.position) <= meleeRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return isInRange && canAttack;
    }

    // is the player near the enemy, and has an attack recently happened?
    private bool CanPerformAttack()
    {
        bool isInRange = Vector3.Distance(transform.position, player.position) >= meleeRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return canAttack && isInRange;
    }

    private void PerformLightMeleeAttack()
    {
        Debug.Log("Performed light melee attack!");
        lastAttackTime = Time.time;
        animator.SetTrigger("meleeLight");
    }

    private void PerformHeavyMeleeAttack()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, 5f * Time.deltaTime);
        animator.SetTrigger("meleeHeavy");
        Debug.Log("Performed heavy melee attack!");
        lastAttackTime = Time.time;
    }

    private void PerformRangedAttack()
    {
        animator.SetTrigger("ranged");
        jumpStartPosition = transform.position;
        jumpTargetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        startTime = Time.time; //to time jumo

        isJumping = true;
    }

    //called by animation event so that damage is taken WHILST attack is done
    public void playerTakeDamage(int damage)
    {
        //is player still in range of enemy when attack occurs?
        if (Vector3.Distance(transform.position, player.position) <= meleeRange)
        {
            Debug.Log(damage);
            ApplyDamageToPlayer(damage);
        }
    }

    private void ApplyDamageToPlayer(int damage)
    {
        // damage player!
        //playerhealth.playerpublicdamage(damage);

    }
}
