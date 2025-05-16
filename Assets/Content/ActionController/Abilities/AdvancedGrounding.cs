using UnityEngine;

[System.Serializable]
public class AdvancedGrounding
{
    private Essentials essentials;

    [Header("ForceMode")]
    public bool withForce = false;
    public float springStrength = 150f;
    public float damping = 15f;
    public float maxSpringForce = 25f;

    public void Init(Essentials _essentials)
    {
        essentials = _essentials;
    }

    public void GroundAlign(float rideHeight, float offsetHeight, float groundSpeed, bool onlyUpwards, bool onlyDownwards, Vector3 groundNormal)
    {
        if (withForce && !onlyUpwards && !onlyDownwards)
        {
            Vector3 forceDir = groundNormal * -1f;
            float forceOffset = offsetHeight - rideHeight;
            float relVel = Vector3.Dot(forceDir, essentials.rigidbody.linearVelocity);
            float springForce = forceOffset * springStrength - damping * relVel;
            springForce = Mathf.Clamp(springForce, -maxSpringForce, maxSpringForce);
            essentials.rigidbody.AddForce(forceDir * springForce);
        }
        else if (onlyUpwards && !onlyDownwards)
        {
            float offset = offsetHeight - rideHeight;
            if (offset < 0f)
            {
                Vector3 targetPos = new Vector3(essentials.rigidbody.transform.position.x, essentials.rigidbody.transform.position.y - offset, essentials.rigidbody.transform.position.z);
                targetPos = Vector3.MoveTowards(essentials.rigidbody.transform.position, targetPos, groundSpeed);
                essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
                essentials.rigidbody.transform.position = targetPos;
            }
        }
        else if (onlyDownwards && !onlyUpwards)
        {
            float offset = offsetHeight - rideHeight;
            if (offset > 0f)
            {
                Vector3 targetPos = new Vector3(essentials.rigidbody.transform.position.x, essentials.rigidbody.transform.position.y - offset, essentials.rigidbody.transform.position.z);
                targetPos = Vector3.MoveTowards(essentials.rigidbody.transform.position, targetPos, groundSpeed);
                essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
                essentials.rigidbody.transform.position = targetPos;
            }
        }
        // Keeps character floating above ground / up and downwards
        else
        {
            float offset = offsetHeight - rideHeight;
            Vector3 targetPos = new Vector3(essentials.rigidbody.transform.position.x, essentials.rigidbody.transform.position.y - offset, essentials.rigidbody.transform.position.z);
            targetPos = Vector3.MoveTowards(essentials.rigidbody.transform.position, targetPos, groundSpeed);
            essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
            essentials.rigidbody.transform.position = targetPos;
        }
    }
}
