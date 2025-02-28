using UnityEngine;

public interface IAgentMover
{
    void Move(Vector3 input, float speed);
    Vector3 CurrentVelocity { get; }
}