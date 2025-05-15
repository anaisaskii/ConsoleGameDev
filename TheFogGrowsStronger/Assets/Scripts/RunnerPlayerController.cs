using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerPlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float FallTimeout = 0.15f;
    public float JumpHeight = 1.2f;

    [Header("Dash (Utility)")]
    public float UpDashPower = 5f;
    public float DashCooldown = 1.0f;
    private bool m_canDash = true;

    [Header("Attack Rates & Prefabs")]
    public Transform firePoint;

    // Primary (Homing Arrow)
    public GameObject homingProjectilePrefab;
    public float homingFireRate = 2f;
    private float nextHomingFireTime;

    // Secondary (Power Shot)
    public GameObject secondaryProjectilePrefab;
    public float secondaryAttackDamage = 50f;
    public float secondaryAttackCooldown = 4f;
    private bool canUseSecondaryAttack = true;

    // Special (UpDash + Rapid Homing)
    public int specialBurstCount = 10;
    public float specialBurstRate = 0.05f;

    [Header("Audio Events")]
    private EventInstance jumpInstance;
    private string jumpSound = "event:/TestSoundEffect";

    // Components & Input
    private CharacterController m_controller;
    private PlayerGameInput m_input;
    private Camera mainCamera;

    private float m_verticalVelocity;
    private float m_fallTimeoutDelta;

    private void Awake()
    {
        m_controller = GetComponent<CharacterController>();
        m_input = GetComponent<PlayerGameInput>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        m_input.OnSecondarySkillInput += HandleSecondarySkillInput;
        m_input.OnUtilitySkillInput += HandleUtilitySkillInput;
        m_input.OnSpecialSkillInput += HandleSpecialSkillInput;
    }

    private void OnDisable()
    {
        m_input.OnSecondarySkillInput -= HandleSecondarySkillInput;
        m_input.OnUtilitySkillInput -= HandleUtilitySkillInput;
        m_input.OnSpecialSkillInput -= HandleSpecialSkillInput;
    }

    private void Update()
    {
        // Movement & Gravity omitted for brevity...
        ApplyGravityAndMovement();

        // PRIMARY: Homing arrows
        if (m_input.PrimarySkillHeld && Time.time >= nextHomingFireTime)
        {
            FireHomingArrow();
            nextHomingFireTime = Time.time + 1f / homingFireRate;
        }
    }

    private void ApplyGravityAndMovement()
    {
        // Your existing movement + gravity implementation
    }

    // ---------- HANDLERS ----------
    private void HandleSecondarySkillInput()
        => StartCoroutine(SecondaryPowerShot());

    private void HandleUtilitySkillInput()
    {
        if (m_canDash)
            StartCoroutine(UpwardDash());
    }
    private void HandleSpecialSkillInput()
        => StartCoroutine(SpecialAttackSequence());

    // ---------- COROUTINES & METHODS ----------

    private IEnumerator SecondaryPowerShot()
    {
        canUseSecondaryAttack = false;
        RuntimeManager.PlayOneShot("event:/Player/Laser_Gun");
        FireProjectile(secondaryProjectilePrefab, secondaryAttackDamage);
        yield return new WaitForSeconds(secondaryAttackCooldown);
        canUseSecondaryAttack = true;
    }

    private IEnumerator UpwardDash()
    {
        m_canDash = false;
        // propel player upwards
        m_controller.Move(Vector3.up * UpDashPower);
        yield return new WaitForSeconds(DashCooldown);
        m_canDash = true;
    }

    private IEnumerator SpecialAttackSequence()
    {
        // 1) Upward dash
        yield return UpwardDash();

        // 2) Rapid homing in air
        for (int i = 0; i < specialBurstCount; i++)
        {
            FireHomingArrow();
            yield return new WaitForSeconds(specialBurstRate);
        }
    }

    private void FireHomingArrow()
    {
        if (homingProjectilePrefab == null || firePoint == null) return;
        // instantiate homing projectile
        GameObject proj = Instantiate(homingProjectilePrefab, firePoint.position, firePoint.rotation);
        HomingProjectile hp = proj.GetComponent<HomingProjectile>();
        if (hp != null)
        {
            hp.speed = proj.GetComponent<HomingProjectile>().speed;
            hp.damage = proj.GetComponent<HomingProjectile>().damage;
        }
    }

    private void FireProjectile(GameObject prefab, float damage)
    {
        if (prefab == null || firePoint == null) return;
        GameObject proj = Instantiate(prefab, firePoint.position, firePoint.rotation);
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
            p.damage = damage;
    }
}

// ---------- HOMING PROJECTILE SCRIPT ----------
public class HomingProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;
    public float rotateSpeed = 200f;
    public LayerMask enemyLayers;

    private Transform target;

    private void Start()
    {
        FindNearestEnemy();
    }

    private void Update()
    {
        if (target == null) { Destroy(gameObject); return; }
        // Seek towards target
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            Health h = other.GetComponent<Health>();
            if (h != null) h.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private void FindNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 50f, enemyLayers);
        float minDist = Mathf.Infinity;
        foreach (var col in hits)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = col.transform;
            }
        }
    }
}
