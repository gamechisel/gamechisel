// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace Milan.ActionControllerSystem
// {
//     public class ActionBounceData
//     {
//         [Header("Bounce")]
//         public float bounceForce = 2f;
//         public float bounceDuration = 0.6f;
//         public float bounceTime;
//     }

//     [System.Serializable]
//     public class ActionBounce : IActionAbility
//     {

//         bool forwardBounce = false;
//         bool verticalBounce = false;
//         bool canForwardBounce = false;
//         bool canUpBounce = false;

//         public override void PerformAction(params object[] parameters) { }

//         public override void PerformNextAction(params object[] parameters) { }

//         public void SetBounce()
//         {

//         }

//         public void DoBounce()
//         {
//             // float speed = rb.velocity.magnitude;
//             // if (isRolling || isSliding && !onSlope && speed > crouchForce)
//             // {
//             //     canForwardBounce = true;
//             // }

//             // if (isJumping || isCrouchJumping)
//             // {
//             //     canUpBounce = true;
//             // }

//             // // Check for collisions horizontal
//             // RaycastHit colHit;
//             // if (canForwardBounce)
//             // {
//             //     foreach (Transform colCheck in colChecks)
//             //     {
//             //         if (Physics.Raycast(colCheck.position, colCheck.forward, out colHit, characterRadius + colRadius, groundLayer))
//             //         {
//             //             float checkAngle = Vector3.Angle(colHit.normal, rb.transform.up);
//             //             if (checkAngle > wallAngle)
//             //             {
//             //                 forwardBounce = true;
//             //             }
//             //         }
//             //     }
//             // }

//             // // Check for collisions vertical
//             // if (canUpBounce)
//             // {
//             //     Transform midPoint = rb.transform;
//             //     if (Physics.Raycast(midPoint.position, rb.transform.up, out colHit, rideHeight + colRadius, groundLayer))
//             //     {
//             //         verticalBounce = true;
//             //     }
//             // }

//             // // Do bounce
//             // if (forwardBounce || verticalBounce)
//             // {

//             //     // Reset State
//             //     isRolling = false;
//             //     isJumping = false;
//             //     isSliding = false;
//             //     rb.velocity = Vector3.zero;

//             //     if (forwardBounce && canStandUp || verticalBounce && !canStandUp && onGround)
//             //     {
//             //         bounceTime = bounceDuration;
//             //         isBouncing = true;
//             //         isAction = true;
//             //         if (forwardBounce)
//             //         {
//             //             rb.velocity = -rb.transform.forward * bounceForce;
//             //         }
//             //     }
//             //     else
//             //     {
//             //         isCrouching = true;
//             //     }
//             // }

//             // if (isBouncing)
//             // {
//             //     bounceTime -= Time.deltaTime;
//             //     if (bounceTime <= 0f)
//             //     {
//             //         isBouncing = false;
//             //     }
//             // }
//         }

//     }
// }x