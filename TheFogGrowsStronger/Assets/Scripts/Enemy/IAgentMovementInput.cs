using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgentMovementInput
{
    public Vector2 MovementInput { get; }

    public bool JumpInput { get; }

    public bool InteractInput { get; }
    public bool ActivateEquipmentInput { get; }

    public bool SprintInput { get; }

    public bool PrimarySkillInput { get; }
    public bool SecondarySkillInput { get; }
    public bool UtilitySkillInput { get; }
    public bool SpecialSkillInput { get; }
}
