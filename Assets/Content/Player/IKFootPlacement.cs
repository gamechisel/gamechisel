using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKFootPlacement : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public TwoBoneIKConstraint ikConstraintLeft;
    public TwoBoneIKConstraint ikConstraintRight;
    public LayerMask layerMask;
    [Range(0, 1f)]
    public float DistanceToGround;
    [Header("IK Weight")]
    public Transform leftFoot;
    public Transform leftFootTarget;
    public Transform rightFoot;
    public Transform rightFootTarget;

    private void LateUpdate()
    {
        float ikleft = anim.GetFloat("IKLeftFootWeight");
        ikConstraintLeft.weight = ikleft;
        float ikright = anim.GetFloat("IKRightFootWeight");
        ikConstraintRight.weight = ikright;

        // Debug.Log(ikleft);
        Debug.Log("RIGHT= " + ikright);

        // Left Foot
        // RaycastHit hit;
        // Ray ray = new Ray(leftFoot.transform.position + Vector3.up, Vector3.down);
        // if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, layerMask))
        // {
        //     Vector3 footPosition = hit.point; // The target foot position is where the raycast hit a walkable object...
        //     footPosition.y += DistanceToGround; // ... taking account the distance to the ground we added above.
        //     leftFootTarget.position = footPosition;
        //     leftFoot.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // The foot rotation is the normal of the hit surface.

        // anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
        // anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        // }

        // Right Foot
        // RaycastHit hit2;
        // Ray ray2 = new Ray(rightFoot.transform.position + Vector3.up, Vector3.down);
        // if (Physics.Raycast(ray2, out hit2, DistanceToGround + 1f, layerMask))
        // {
        //     Vector3 footPosition = hit2.point;
        //     footPosition.y += DistanceToGround;
        //     rightFootTarget.position = footPosition;
        //     rightFoot.rotation = Quaternion.FromToRotation(Vector3.up, hit2.normal);
        // }
    }

}