using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FootIKController : MonoBehaviour
{
    public TwoBoneIKConstraint ikConstraint;  // Referenz zu deinem IK Constraint
    public Transform footTransform;            // Transform des Fußknochens (z. B. RightFoot)
    public Transform footTarget;               // Transform des Fuß-Ziels (z. B. FootTarget)
    public LayerMask groundLayer;              // Layer für den Boden
    public float stepHeight = 0.3f;           // Schwellenwert für die Schritthöhe
    public float maxGroundHitDistance = 0.5f;           // Maximale Distanz, bei der IK aktiv wird
    public float maxDiffDistance = 0.3f;     // Maximale Distanz, bei der das Target noch als gültig betrachtet wird
    public float blendSpeed = 15f;              // Geschwindigkeit für das Anpassen des IK-Gewichts
    Vector3 lastHitPoint = Vector3.zero;

    private void LateUpdate()
    {
        // Fuß-Target positionieren und IK-Gewicht dynamisch anpassen
        UpdateFootTargetPosition();
    }

    private void UpdateFootTargetPosition()
    {
        // Raycast vom Fuß nach unten, um den Boden zu finden
        RaycastHit hit;
        if (Physics.Raycast(footTransform.position + Vector3.up * stepHeight, Vector3.down, out hit, maxGroundHitDistance, groundLayer))
        {
            // float distance = Vector3.Distance(hit.point, lastHitPoint);
            // if (distance >= maxDiffDistance)
            // {
            lastHitPoint = hit.point;
            footTarget.position = hit.point;
            footTarget.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            // }
            // ikConstraint.weight = Mathf.Lerp(ikConstraint.weight, 1f, Time.deltaTime * blendSpeed);
            ikConstraint.weight = 1f;

        }
        else
        {
            // Wenn der Fuß den Boden verlässt oder zu weit entfernt ist
            // Setze das Target auf die Fußposition und deaktiviere das IK (Gewicht auf 0)
            footTarget.position = footTransform.position;
            // ikConstraint.weight = Mathf.Lerp(ikConstraint.weight, 0f, Time.deltaTime * blendSpeed);
            ikConstraint.weight = 0f;
        }
    }
}
