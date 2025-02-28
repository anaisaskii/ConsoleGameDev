using Cinemachine;
using ECM2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Cached Character

    private Character _character;



    private void Awake()
    {
        // Cache our controlled character

        _character = GetComponent<Character>();
    }

    private void Start()
    {
        // Lock mouse cursor

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Movement input

        Vector2 inputMove = new Vector2()
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = Input.GetAxisRaw("Vertical")
        };

        // Set Movement direction in world space

        Vector3 movementDirection = Vector3.zero;

        movementDirection += Vector3.right * inputMove.x;
        movementDirection += Vector3.forward * inputMove.y;

        // If character has a camera assigned...

        if (_character.camera)
        {
            // Make movement direction relative to its camera view direction

            movementDirection = movementDirection.relativeTo(_character.cameraTransform);
        }

        // Set Character movement direction

        _character.SetMovementDirection(movementDirection);


        // Jump input

        if (Input.GetButtonDown("Jump"))
            _character.Jump();
        else if (Input.GetButtonUp("Jump"))
            _character.StopJumping();

    }
}
