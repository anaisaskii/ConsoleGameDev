using ECM2;
using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    protected IAgentMover m_mover;

    protected IAgentMovementInput m_input;

    private Character m_character;

    private void Awake()
    {
        m_input = GetComponent<IAgentMovementInput>();

        m_mover = GetComponent<IAgentMover>();

        m_character = GetComponent<Character>();
    }

    private void Update()
    {
        Vector3 movementDirection = Vector3.zero;

        movementDirection += Vector3.right * m_input.MovementInput.x;
        movementDirection += Vector3.forward * m_input.MovementInput.y;

        m_character.SetMovementDirection(movementDirection);
    }

}
