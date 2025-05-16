using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Static Input Utilities
public static class InputUtils
{
    public static Vector3 ProjectDirection(Vector3 currentDirection, Vector3 transformAxis)
    {
        if (transformAxis != null)
        {
            return Vector3.ProjectOnPlane(currentDirection, transformAxis);
        }
        return Vector3.ProjectOnPlane(currentDirection, Vector3.up);
    }

    // Returns MoveDirection based on Camera angle
    public static Vector3 MoveDirection(Transform cameraTransform, Vector2 moveInput, float moveAmount)
    {
        Vector3 moveDirection = cameraTransform.forward * moveInput.y +
            cameraTransform.right * moveInput.x;
        moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
        moveDirection = moveDirection.normalized * moveAmount;
        return moveDirection;
    }

    // Returns Camera Direction / Projected
    public static Vector3 LookDirection(Transform cameraTransform, bool projected)
    {
        Vector3 lookDirection = cameraTransform.forward;
        if (projected)
        {
            lookDirection = Vector3.ProjectOnPlane(lookDirection, Vector3.up);
        }
        return lookDirection;
    }

    // private Essentials essentials;
    // private float changeSpeed;
    // private float turnSpeed = 1f;

    // Returns Turn Direction
    // public Vector3 ChangeDirection(Vector3 wantDirection, Transform cameraTransform)
    // {
    //     Vector3 changeDirection = wantDirection;
    //     Vector3 velCheck = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
    //     if (velCheck.magnitude > changeSpeed)
    //     {
    //         changeDirection = Vector3.Lerp(velCheck, changeDirection, turnSpeed);
    //     }
    //     return changeDirection;
    // }
}