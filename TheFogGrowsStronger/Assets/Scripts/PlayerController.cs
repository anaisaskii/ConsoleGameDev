using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float FallTimeout = 0.15f;

    private CharacterController m_controller;
    private IAgentMovementInput m_input;
    private float m_speed;
    private float m_targetRotation = 0.0f;
    private float m_rotationVelocity;
    private float m_verticalVelocity;
    private float m_fallTimeoutDelta;

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

    private Animator m_animator;
    private float m_animationMovementSpeed;
    //private GameObject _mainCamera;

    [SerializeField]
    private AgentRotationStrategy m_rotationStrategy;

    //Getting reference to Unity specific objects
    private void Awake()
    {
        // get a reference to the RotationStrategy to help us separate Agent script from depending on the Camera
        if (m_rotationStrategy == null)
        {
            m_rotationStrategy = GetComponent<AgentRotationStrategy>();
        }
        m_animator = GetComponent<Animator>();
        m_controller = GetComponent<CharacterController>();
        //Now IAgentMovementInput handles communication with the InputSystem
        m_input = GetComponent<IAgentMovementInput>();
    }

    //All the logic connected with movement happens in the Update
    private void Update()
    {
        if (Grounded == false)
        {
            m_verticalVelocity += Gravity * Time.deltaTime;
            m_fallTimeoutDelta -= Time.deltaTime;
            if (m_fallTimeoutDelta <= 0 && StairsGrounded == false)
            {
                m_animator.SetTrigger(AnimationFallTrigger);
            }
        }
        else
        {
            m_verticalVelocity = 0;
            m_fallTimeoutDelta = FallTimeout;
            m_animator.ResetTrigger(AnimationFallTrigger);
        }

        CharacterMovementCalculation();

        m_targetRotation = m_rotationStrategy.RotationCalculation(m_input.MovementInput, transform, ref m_rotationVelocity, RotationSmoothTime, m_targetRotation);

        Vector3 targetDirection = Quaternion.Euler(0.0f, m_targetRotation, 0.0f) * Vector3.forward;

        //move the character controller
        m_controller.Move(targetDirection.normalized * (m_speed * Time.deltaTime) +
                         new Vector3(0.0f, m_verticalVelocity, 0.0f) * Time.deltaTime);

        //play animations
        m_animator.SetFloat(AnimationSpeedFloat, m_animationMovementSpeed);
    }

    //[SerializeField]
    //private bool isPlayer = false;
    //[SerializeField]
    //private GameObject _mainCamera;
    //private void RotationCalculation()
    //{
    //    // normalize input direction
    //    Vector3 inputDirection = new Vector3(_input.MovementInput.x, 0.0f, _input.MovementInput.y).normalized;

    //    //Rotation Code
    //    if (_input.MovementInput != Vector2.zero)
    //    {
    //        if (isPlayer)
    //        {
    //            // Player rotation logic
    //            _targetRotation = Mathf.Atan2(inputDirection.x,
    //                              inputDirection.z) * Mathf.Rad2Deg +
    //                              _mainCamera.transform.eulerAngles.y;
    //        }
    //        else
    //        {
    //            // NPC rotation logic 
    //            _targetRotation = Mathf.Atan2(inputDirection.x,
    //                              inputDirection.z) * Mathf.Rad2Deg;
    //        }

    //        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
    //            RotationSmoothTime);

    //        // rotate to face input direction relative to camera position
    //        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    //    }
    //}

    //Calculating the movement speed and controlling the MOVEMENT animation
    private void CharacterMovementCalculation()
    {
        float targetSpeed = m_input.SprintInput ? SprintSpeed : MoveSpeed;


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
            m_animationMovementSpeed = 0f;
    }

    //Grounded checks that allows us to swap between FALL and MOVEMENT animation / behavior
    private void FixedUpdate()
    {
        Grounded = GroundedCheck(GroundedOffset);
        StairsGrounded = GroundedCheck(StairOffset);
        m_animator.SetBool(AnimationGroundedBool, Grounded);
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
