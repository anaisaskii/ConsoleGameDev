using UnityEngine;
using UnityEngine.InputSystem;
using System;

//Input Management
public class PlayerGameInput : MonoBehaviour
{
    private PlayerInput m_input;

    public event Action OnPrimarySkillInput;
    public event Action OnSecondarySkillInput;
    public event Action OnUtilitySkillInput;
    public event Action OnSpecialSkillInput;

    public Vector2 MovementInput { get; private set; }
    public Vector2 CameraInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool PrimarySkillHeld { get; private set; }

    private void Awake()
    {
        m_input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        m_input.actions["Player/Move"].performed += OnMove;
        m_input.actions["Player/Move"].canceled += OnMove;

        m_input.actions["Player/Jump"].performed += OnJump;
        m_input.actions["Player/Jump"].canceled += OnJump;

        m_input.actions["Player/PrimarySkill"].performed += OnPrimarySkill;
        m_input.actions["Player/PrimarySkill"].canceled += OnPrimarySkillCanceled;

        m_input.actions["Player/SecondarySkill"].performed += OnSecondarySkill;

        m_input.actions["Player/ToggleSprint"].performed += OnToggleSprint;

        m_input.actions["Player/UtilitySkill"].performed += OnUtilitySkill;

        m_input.actions["Player/SpecialSkill"].performed += OnSpecialSkill;
    }

    private void OnDisable()
    {
        m_input.actions["Player/Move"].performed -= OnMove;
        m_input.actions["Player/Move"].canceled -= OnMove;

        m_input.actions["Player/Jump"].performed -= OnJump;
        m_input.actions["Player/Jump"].canceled -= OnJump;

        m_input.actions["Player/PrimarySkill"].performed -= OnPrimarySkill;
        m_input.actions["Player/PrimarySkill"].canceled -= OnPrimarySkillCanceled;

        m_input.actions["Player/SecondarySkill"].performed -= OnSecondarySkill;

        m_input.actions["Player/ToggleSprint"].performed -= OnToggleSprint;

        m_input.actions["Player/UtilitySkill"].performed -= OnUtilitySkill;

        m_input.actions["Player/SpecialSkill"].performed -= OnSpecialSkill;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        CameraInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpInput = context.ReadValueAsButton();
    }

    public void OnToggleSprint(InputAction.CallbackContext context)
    {
        SprintInput = !SprintInput;
    }

    public void OnUtilitySkill(InputAction.CallbackContext context)
    {
        OnUtilitySkillInput?.Invoke();
    }

    public void OnPrimarySkill(InputAction.CallbackContext context)
    {
        PrimarySkillHeld = true;
        OnPrimarySkillInput?.Invoke();
    }

    public void OnSecondarySkill(InputAction.CallbackContext context)
    {
        OnSecondarySkillInput?.Invoke();
    }

    public void OnSpecialSkill(InputAction.CallbackContext context)
    {
        OnSpecialSkillInput?.Invoke();
    }

    private void OnPrimarySkillCanceled(InputAction.CallbackContext context)
    {
        PrimarySkillHeld = false;
    }
}
