using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles Spring Function, for example player grounding or vehicle suspension for wheels

public class Spring : MonoBehaviour
{
    [Header("Spring Components")]
    private Rigidbody rb;
    private Vector3 springDirection;
    private float springStrength;
    private float springDamper;
    private Transform point;

    public Spring(Rigidbody _rb, Vector3 _sD)
    {
        rb = _rb;
        springDirection = _sD;
    }

    public void ApplyForce(float offset)
    {
        // Object Velocity
        Vector3 stabilizerWorldVel = rb.GetPointVelocity(point.position);

        // Get Velocity in Spring Direction
        float vel = Vector3.Dot(springDirection, stabilizerWorldVel);

        // Spring Force
        float force = (offset * springStrength) - (vel * springDamper);

        // Spring Direction
        Vector3 suspensionForce = springDirection * force;

        // Apply force to the tire for suspension
        rb.AddForceAtPosition(suspensionForce, point.position);
        Debug.DrawRay(point.position, suspensionForce, Color.green);
    }
}
