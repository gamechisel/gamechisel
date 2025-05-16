using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionRotationData
{
    [Header("Rotation Data")]
    [Tooltip("Rotate speed in deegres per second.")]
    [Range(1f, 360f)]
    public float rotateSpeed = 15f; // best balance
    [Tooltip("Ignore small velocity rotation.")]
    public float minRotationSpeed = 0.3f;
    public float turnSpeed = 2f;
}

[System.Serializable]
public class ActionRotation
{
    public ActionRotationData data;
    private Rigidbody rigidbody; // only rigidbody for reusability

    public void Init(Rigidbody _rigidbody)
    {
        rigidbody = _rigidbody;
    }

    public void SetValues(ActionRotationData _data)
    {
        data = _data;
    }

    public void RotateToVelocity()
    {
        Vector3 vel = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
        Vector3 lookDirection = vel;
        if (lookDirection != Vector3.zero && lookDirection.magnitude > data.minRotationSpeed)
        {
            lookDirection = Vector3.Normalize(lookDirection);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            Quaternion playerRotation = Quaternion.Slerp(rigidbody.transform.rotation, lookRotation, data.rotateSpeed * Time.deltaTime);
            rigidbody.transform.rotation = playerRotation;
        }
    }

    public void RotateToDirection(Vector3 dir)
    {
        Vector3 lookDirection = dir;
        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            Quaternion playerRotation = Quaternion.RotateTowards(rigidbody.transform.rotation, lookRotation, data.rotateSpeed);
            rigidbody.transform.rotation = playerRotation;
        }
    }

    public Vector3 LimitRotation(Vector3 currentVelDir, Vector3 wantDir, float changeDirectionAmount)
    {
        // Calculate the rotation difference between currentVelDir and wantDir
        Quaternion fromRotation = Quaternion.LookRotation(currentVelDir);
        Quaternion toRotation = Quaternion.LookRotation(wantDir);
        Quaternion rotationDifference = toRotation * Quaternion.Inverse(fromRotation);

        // Convert the rotation difference to an angle-axis representation
        float angle;
        Vector3 axis;
        rotationDifference.ToAngleAxis(out angle, out axis);

        // calculate percentual value
        float distance = Vector3.Distance(currentVelDir, wantDir);
        float percentualChange = 1f;
        if (distance > changeDirectionAmount)
        {
            percentualChange = changeDirectionAmount / distance;
        }

        // Calculate the maximum allowed rotation angle based on changeDirectionAmount
        float maxRotationAngle = 180f * percentualChange; // Percentual value

        // If the angle is greater than the maximum allowed rotation angle, limit it
        if (angle > maxRotationAngle)
        {
            angle = maxRotationAngle;
        }

        // Apply the limited rotation
        Quaternion limitedRotation = Quaternion.AngleAxis(angle, axis);
        Quaternion newRotation = fromRotation * limitedRotation;

        // Convert the new rotation to a direction vector
        Vector3 newDir = newRotation * Vector3.forward;

        return newDir;
    }

    public void FocusTarget(Transform targetTransform)
    {
        // Berechne die Richtung zum Ziel
        Vector3 directionToTarget = targetTransform.position - rigidbody.transform.position;

        // Berechne die Rotation, die zum Ziel führt
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Setze die Rotation des Spielers auf die berechnete Zielrotation
        rigidbody.transform.rotation = targetRotation;
    }

}