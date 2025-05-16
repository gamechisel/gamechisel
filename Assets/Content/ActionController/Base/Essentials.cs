using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Controller Essentials Values for character stats like weight

// namespace Milan.ActionControllerSystem
// {
[System.Serializable]
public class Essentials
{
    [Header("Base Variables")]
    public Rigidbody rigidbody; // rigidbody to be moved
    public Transform midPoint; // center of character
    public float rideHeight; // half height of character
    public float characterRadius; // radius of character collider, wallcheck
    public float swimHeight; // height of low shoulders - character radius
    public float headHeight; // height of head - character radius
    public float stairHeight; // character floating height
    public float weight; // character weight for platforms

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask waterLayer;

    [Header("Velocity")]
    public float horizontalVelocity;
    public float verticalVelocity;

    public float HorizontalVelocity()
    {
        return horizontalVelocity = Vector3.ProjectOnPlane(rigidbody.linearVelocity, Vector3.up).magnitude;
    }

    public float VerticalVelocity()
    {
        return verticalVelocity = rigidbody.linearVelocity.y;
    }

    public float SideVelocity()
    {
        Vector3 rightDirection = rigidbody.transform.right; // Replace with your object's right direction

        // Calculate the side velocity using dot product
        float sideVelocity = Vector3.Dot(rigidbody.linearVelocity, rightDirection);

        return sideVelocity;
    }

    public float ForwardVelocity()
    {
        Vector3 forwardDirection = rigidbody.transform.forward; // Replace with your object's forward direction

        // Calculate the forward velocity using dot product
        float forwardVelocity = Vector3.Dot(rigidbody.linearVelocity, forwardDirection);

        return forwardVelocity;
    }
}
// }