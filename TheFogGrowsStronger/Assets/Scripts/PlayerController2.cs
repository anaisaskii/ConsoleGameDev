using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class PlayerController2 : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float FallTimeout = 0.15f;
    public float JumpHeight = 1.2f;
    public float DashDistance = 5.0f;
    public float DashCooldown = 1.0f;

    private CharacterController m_controller;
    private PlayerGameInput m_input;
    private float m_speed;
    private float m_targetRotation = 0.0f;
    private float m_rotationVelocity;
    private float m_verticalVelocity;
    private float m_fallTimeoutDelta;
    private bool m_canDash = true;

    [Header("Grounded Check")]
    public float Gravity = -15.0f;
    public bool Grounded = true, StairsGrounded = true;
    public float GroundedOffset = 0.21f, StairOffset = 0.07f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("Animations")]
    public string AnimationSpeedFloat;
    public string AnimationGroundedBool;
    public string AnimationFallTrigger;

    [Header("Rotation Override Settings")]
    [Tooltip("How long (in seconds) to keep character facing the camera after an attack.")]
    public float AttackRotationDuration = 5f;

    [Header("Haptics")]
    [Tooltip("Low (left motor) / High (right motor) rumble strength (0…1).")]
    public float rumbleLow = 0.2f;
    public float rumbleHigh = 0.5f;
    [Tooltip("How long the rumble should play in seconds.")]
    public float rumbleDuration = 0.1f;
    private Coroutine _rumbleCoroutine;

    private bool _attackLock = false;
    private float _attackLockTimer = 0f;

    public Animator m_animator;
    private float m_animationMovementSpeed;

    [SerializeField]
    private AgentRotationStrategy m_rotationStrategy;

    public int cash = 0;
    public TextMeshProUGUI cashText;

    [Header("Shooting")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 3f;
    public float projectileSpeed = 30f;
    public float projectileDamage = 10f;
    public LayerMask shootableLayers;
    public float shootDistance = 100f;
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffectPrefab;

    private float nextFireTime;

    private Camera mainCamera;

    public GameObject normalProjectilePrefab;
    public GameObject secondaryProjectilePrefab;
    public GameObject specialProjectilePrefab;

    public float normalAttackDamage = 10f;
    public float secondaryAttackDamage = 50f;
    public float secondaryAttackCooldown = 4f;
    public float specialAttackBurstRate = 0.05f; // Faster burst rate
    public int specialAttackBurstCount = 10;

    private bool canUseSecondaryAttack = true;

    private EventInstance jumpInstance;
    private string jumpInstanceLocation = "event:/TestSoundEffect";

    private void OnEnable()
    {
        m_input.OnPrimarySkillInput += HandlePrimarySkillInput;
        m_input.OnSecondarySkillInput += HandleSecondarySkillInput;
        m_input.OnUtilitySkillInput += HandleUtilitySkillInput;
        m_input.OnSpecialSkillInput += HandleSpecialSkillInput;
    }

    private void OnDisable()
    {
        m_input.OnPrimarySkillInput -= HandlePrimarySkillInput;
        m_input.OnSecondarySkillInput -= HandleSecondarySkillInput;
        m_input.OnUtilitySkillInput -= HandleUtilitySkillInput;
        m_input.OnSpecialSkillInput -= HandleSpecialSkillInput;
    }

    //reference to player follow cam
    //use for FOV and other effects
    public CinemachineFreeLook freelookCam;

    private Vector3 aimPoint;

    private void BeginAttackLock()
    {
        _attackLock = true;
        _attackLockTimer = 0f;
    }

    private void Awake()
    {
        if (m_rotationStrategy == null)
        {
            m_rotationStrategy = GetComponent<AgentRotationStrategy>();
        }
        //m_animator = GetComponent<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_input = GetComponent<PlayerGameInput>();
        mainCamera = Camera.main;
    }

    private void HandleSecondarySkillInput()
    {
        if (canUseSecondaryAttack)
        {
            StartCoroutine(SecondaryAttack());
        }
    }

    private void HandleSpecialSkillInput()
    {
        StartCoroutine(SpecialAttack());
    }

    private IEnumerator SecondaryAttack()
    {

        canUseSecondaryAttack = false;
        FireProjectile(secondaryProjectilePrefab, secondaryAttackDamage);
        yield return new WaitForSeconds(secondaryAttackCooldown);
        canUseSecondaryAttack = true;
    }

    private IEnumerator SpecialAttack()
    {
        for (int i = 0; i < specialAttackBurstCount; i++)
        {
            FireProjectile(specialProjectilePrefab, normalAttackDamage);
            yield return new WaitForSeconds(specialAttackBurstRate);
        }
    }

    private void HandlePrimarySkillInput()
    {
        // The shooting logic is handled in Update since we want continuous fire
    }

    private void HandleUtilitySkillInput()
    {
        if (m_canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        m_verticalVelocity = Mathf.Sqrt(4 * -2f * Gravity);
        m_canDash = false;
        m_controller.Move(Vector3.up * (m_verticalVelocity * Time.deltaTime));
        yield return new WaitForSeconds(DashCooldown);
        m_canDash = true;
    }

    //All the logic connected with movement happens in the Update
    private void Update()
    {


        cash = cashText.text == "" ? 0 : int.Parse(cashText.text);
        //Debug.Log(cash);

        if (Grounded == false)
        {
            m_verticalVelocity += Gravity * Time.deltaTime;
            m_fallTimeoutDelta -= Time.deltaTime;
            if (m_fallTimeoutDelta <= 0 && StairsGrounded == false)
            {
                //m_animator.SetTrigger(AnimationFallTrigger);
            }
        }
        else
        {
            m_verticalVelocity = 0;
            m_fallTimeoutDelta = FallTimeout;
            //m_animator.ResetTrigger(AnimationFallTrigger);
        }

        CharacterMovementCalculation();

        // 1) Handle the attack-lock timer (as before) …
        if (_attackLock)
        {
            _attackLockTimer += Time.deltaTime;
            if (_attackLockTimer >= AttackRotationDuration)
                _attackLock = false;
        }

        // 2) Compute model rotation (as you already have it):
        Quaternion desiredRot;
        if (_attackLock)
        {
            float camY = mainCamera.transform.eulerAngles.y;
            desiredRot = Quaternion.Euler(0, camY, 0);
        }
        else if (m_input.MovementInput.sqrMagnitude > 0.01f)
        {
            // your old input-based yaw + smoothing here…
            float rawYaw = Mathf.Atan2(m_input.MovementInput.x,
                                      m_input.MovementInput.y)
                           * Mathf.Rad2Deg
                         + mainCamera.transform.eulerAngles.y;
            desiredRot = Quaternion.Euler(0, rawYaw, 0);
        }
        else
        {
            desiredRot = transform.rotation;
        }

        // smooth-slerp into place over RotationSmoothTime
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            Time.deltaTime / RotationSmoothTime
        );

        // 3) BUILD YOUR MOVEMENT FROM INPUT + CAMERA, **NOT** transform.forward:
        Vector3 inputDir = new Vector3(
            m_input.MovementInput.x,
            0f,
            m_input.MovementInput.y
        );
        // rotate that input vector by camera yaw
        Quaternion camYawQ = Quaternion.Euler(
            0f,
            mainCamera.transform.eulerAngles.y,
            0f
        );
        Vector3 moveDir = camYawQ * inputDir;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 horizontalMove = moveDir * m_speed * Time.deltaTime;
        Vector3 verticalMove = Vector3.up * (m_verticalVelocity * Time.deltaTime);

        m_controller.Move(horizontalMove + verticalMove);

        // Handle shooting
        if (m_input.PrimarySkillHeld && Time.time >= nextFireTime)
        {
            FireProjectile(normalProjectilePrefab, normalAttackDamage);
            nextFireTime = Time.time + 1f / fireRate;
        }

        // Handle jumping
        if (m_input.JumpInput && Grounded || m_input.JumpInput && StairsGrounded)
        {
            m_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

            //fmod here
            jumpInstance = RuntimeManager.CreateInstance(jumpInstanceLocation);
            jumpInstance.start();
            jumpInstance.release();
        }

        //m_animator.SetFloat(AnimationSpeedFloat, m_animationMovementSpeed);

    }

    private IEnumerator StopRumbleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        // pause or reset—reset does a hard stop
        Gamepad.current?.PauseHaptics();      // or ResetHaptics()
    }

    private void FireProjectile(GameObject projectilePrefab, float damage)
    {
        BeginAttackLock();

        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(rumbleLow, rumbleHigh);
            // stop any existing coroutine to avoid overlap
            if (_rumbleCoroutine != null) StopCoroutine(_rumbleCoroutine);
            _rumbleCoroutine = StartCoroutine(StopRumbleAfter(rumbleDuration));
        }

        // Calculate aim point in the center of the screen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 aimPoint = ray.GetPoint(shootDistance);

        // Create projectile
        if (firePoint != null && projectilePrefab != null)
        {
            // Spawn muzzle flash
            if (muzzleFlashPrefab != null)
            {
                GameObject muzzleVFX = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
                Destroy(muzzleVFX, 2f); // Destroy after effect duration
            }

            // First do hitscan detection
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, (aimPoint - firePoint.position).normalized, out hit, shootDistance, shootableLayers))
            {
                // Apply damage immediately (hitscan)
                Health health = hit.collider.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }

                // Create hit effect at impact point
                if (hitEffectPrefab != null)
                {
                    GameObject hitVFX = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hitVFX, 2f); // Destroy after effect duration
                }

                // Adjust aim point to hit location for visual projectile
                aimPoint = hit.point;
            }

            // Spawn visual projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.transform.LookAt(aimPoint);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.speed = projectileSpeed;
                projectileScript.damage = 0; // No damage since we already applied it with hitscan
            }
            else
            {
                // If no Projectile component, just make it move forward
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = (aimPoint - firePoint.position).normalized * projectileSpeed;
                }
            }
        }
    }

    //Calculating the movement speed and controlling the MOVEMENT animation
    private void CharacterMovementCalculation()
    {
        float targetSpeed = m_input.SprintInput ? SprintSpeed : MoveSpeed;

        // Modify the FOV manually
        freelookCam.m_Lens.FieldOfView = (targetSpeed == SprintSpeed) ? 65f : 60f;

        if (m_input.MovementInput == Vector2.zero)
            targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(m_controller.velocity.x, 0.0f, m_controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = m_input.MovementInput.magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            m_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            m_speed = Mathf.Round(m_speed * 1000f) / 1000f;
        }
        else
        {
            m_speed = targetSpeed;
        }

        m_animationMovementSpeed = Mathf.Lerp(m_animationMovementSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (m_animationMovementSpeed < 0.01f)
        {
            m_animator.SetBool("Walking", false);
            m_animationMovementSpeed = 0f;
        }

        bool isWalking = m_speed > 0.1f; // If moving, set walking to true
        m_animator.SetBool("Walking", isWalking);
    }

    //Grounded checks that allows us to swap between FALL and MOVEMENT animation / behavior
    private void FixedUpdate()
    {
        Grounded = GroundedCheck(GroundedOffset);
        StairsGrounded = GroundedCheck(StairOffset);
        //m_animator.SetBool(AnimationGroundedBool, Grounded);
    }

    //Spherecasting downwards to detect if we are grounded
    private bool GroundedCheck(float groundedOffset)
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundedOffset,
            transform.position.z);
        return Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }


    //Visualizaton of the Grounded check
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y + GroundedOffset, transform.position.z),
            GroundedRadius);
    }
}
