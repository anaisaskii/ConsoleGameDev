using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentRotationStrategy : MonoBehaviour
{
    //This method defines a common code that each rotation strategy will uses (like the use of SmoothDampAngle(..) )
    public float RotationCalculation(Vector2 movementInput, Transform agentTransform, ref float rotationVelocity, float RotationSmoothTime, float targetRotation)
    {
        Vector3 inputDirection = new Vector3(movementInput.x, 0.0f, movementInput.y).normalized;
        if (movementInput != Vector2.zero)
        {

            targetRotation = RotationStrategy(inputDirection);
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            agentTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        return targetRotation;
    }

    //Each specific implementation will define its own rotation strategy based on input
    protected abstract float RotationStrategy(Vector3 inputDirection);
}