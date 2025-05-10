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
    private GameObject player;
    private Transform playerTransform;

    private Health playerhealth;

    private float meleeRange = 5.0f; // how near the player has to be to attack
    private float combinedCooldown = 6f; // a joint cooldown so attacks can't be spammed
    private float lastAttackTime; // check when the enemy last attacked

    private Root aiRoot; // root for behaviour tree

    //private Animator animator;

    private bool isJumping; // check if jumping
    private float jumpDuration = 1f; // how long does the enemy take to jump
    private float startTime; //start time of jump

    private Vector3 jumpStartPosition;
    private Vector3 jumpTargetPosition;

    public void Start()
    {
        //find player gameobject
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        playerhealth = player.GetComponent<Health>();

        Debug.Log(player);

        //animator = this.GetComponent<Animator>();
        //animator.speed = 0.9f; // 1 was too fast...

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
                            Debug.Log("Selecting attack");
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
        //Debug.Log("Progressing!");
        aiRoot.Tick();
    }

    private bool IsPlayerInMeleeRange()
    {
        bool isInRange = Vector3.Distance(transform.position, playerTransform.position) <= meleeRange;
        bool canAttack = Time.time >= lastAttackTime + combinedCooldown;
        return isInRange && canAttack;
    }

    // is the player near the enemy, and has an attack recently happened?
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
        //animator.SetTrigger("meleeLight");
    }

    private void PerformHeavyMeleeAttack()
    {
        transform.position = Vector3.Lerp(transform.position, playerTransform.position, 5f * Time.deltaTime);
        //animator.SetTrigger("meleeHeavy");
        Debug.Log("Performed heavy melee attack!");
        lastAttackTime = Time.time;
    }

    private void PerformRangedAttack()
    {
        //animator.SetTrigger("ranged");
        jumpStartPosition = transform.position;
        jumpTargetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        startTime = Time.time; //to time jumo
        Debug.Log("Performed ranged attack!");
        isJumping = true;
    }

    //called by animation event so that damage is taken WHILST attack is done
    public void playerTakeDamage(int damage)
    {
        //is player still in range of enemy when attack occurs?
        if (Vector3.Distance(transform.position, playerTransform.position) <= meleeRange)
        {
            Debug.Log(damage);
            ApplyDamageToPlayer(damage);
        }
    }

    private void ApplyDamageToPlayer(int damage)
    {
        // damage player!
        playerhealth.TakeDamage(damage);

    }
}
