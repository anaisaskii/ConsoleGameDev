using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgentMovementInput
{
    public Vector2 MovementInput { get; }
    public bool SprintInput { get; }
}
