using UnityEngine;

/// <summary>
/// Interface for providing movement input to an agent.
/// </summary>
public interface IAgentMovementInput
{
    public Vector2 MovementInput { get; }
    public bool SprintInput { get; }
}
