using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ledge
[System.Serializable]
public class ActionLedge
{
    [Header("Reference")]
    private Essentials essentials;
    private ActionAnimation actionAnimation;
    private PlayerActionState state;
    private ActionSound actionSound;
    private Surroundings surroundings;
    private PlayerActionInputProvider actionInput;

    [Header("Ledge")]
    private float ledgeHeight = 0.5f;
    private float ledgeCheckHeight = 1f;
    private float disableLedgeDuration = 0.5f;
    // forces
    private float pushAwayForce = 1f;
    private float ledgeUpDuration = 0.4f;
    private float ledgeUpForce = 8f;
    private float ledgeForwardForce = 4f;
    private float minLedgeTime = 0.3f;
    private float ledgeTime;
    private float ledgeUpTime;
    // Platform
    private bool ledgePlatform;
    private Vector3 ledgePos;
    private float disLedgeTime;
    // ahhh
    private bool onLedge;
    private bool isLedgeUpJumping;
    public Transform ledgeHolder;
    public Transform[] ledgeChecks;
    public float standAngle = 40f;


    [Header("Ledge")]
    [SerializeField] private AnimationCurve ledgeUpCurve;

    public void Init(Essentials _essentials, ActionSound _actionSound, ActionAnimation _actionAnimation, PlayerActionState _state, Surroundings _surroundings, PlayerActionInputProvider _actionInput)
    {
        essentials = _essentials;
        actionAnimation = _actionAnimation;
        state = _state;
        actionSound = _actionSound;
        surroundings = _surroundings;
        actionInput = _actionInput;
    }

    public void TryLedge()
    {
        // check if something is above the head
        bool _h = Physics.CheckCapsule(essentials.rigidbody.transform.position, essentials.rigidbody.transform.position + essentials.rigidbody.transform.up * ledgeCheckHeight, essentials.characterRadius * 0.9f, essentials.groundLayer);


        //     if (!inWater && essentials.rigidbody.velocity.y <= 0f && !onGround && !onLedge && canStandUp && wantMove && !_h && _a)
        //     {
        //         canLedge = true;
        //     }
        //     else
        //     {
        //         canLedge = false;
        //     }

        bool canLedge = !_h;

        ledgeHolder.position = essentials.rigidbody.transform.position;
        if (actionInput.moveDir != Vector3.zero && !onLedge)
        {
            ledgeHolder.rotation = Quaternion.LookRotation(actionInput.moveDir);
        }

        if (onLedge)
        {
            ledgeHolder.rotation = essentials.rigidbody.transform.rotation;
        }

        if (onLedge && ledgeTime <= minLedgeTime)
        {
            ledgeTime += Time.deltaTime;
        }

        RaycastHit ledgeHit;
        bool hit = false;
        Vector3 pos = Vector3.zero;

        if (canLedge || onLedge)
        {
            foreach (Transform ledgeCheck in ledgeChecks)
            {
                if (Physics.Raycast(ledgeCheck.position, -essentials.rigidbody.transform.up, out ledgeHit, ledgeHeight, essentials.groundLayer))
                {
                    if (canLedge && !onLedge)
                    {
                        if (!hit)
                        {
                            float checkAngle = Vector3.Angle(essentials.rigidbody.transform.up, ledgeHit.normal);
                            if (checkAngle <= standAngle)
                            {
                                ledgeTime = 0f;

                                // Set wanted height
                                float h = ledgeHit.point.y;
                                h -= essentials.rideHeight;

                                essentials.rigidbody.transform.position = new Vector3(essentials.rigidbody.transform.position.x, h, essentials.rigidbody.transform.position.z);
                                ledgeHolder.position = essentials.rigidbody.transform.position;

                                hit = true;

                                Vector3 dir = ledgeHit.point - essentials.rigidbody.transform.position;
                                dir = Vector3.ProjectOnPlane(dir, essentials.rigidbody.transform.up);
                                essentials.rigidbody.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(dir));
                                ledgeHolder.rotation = essentials.rigidbody.transform.rotation;

                                // Reset values
                                onLedge = true;
                                state.SetActionState("ledge", 0.04f);

                                // Zero velocity
                                essentials.rigidbody.linearVelocity = Vector3.zero;

                                //                             if (ledgeHit.transform.gameObject.tag == "Platform")
                                //                             {
                                //                                 platform = ledgeHit.transform;
                                //                                 ledgePlatform = true;
                                //                                 platLastPosition = platform.position;
                                //                                 platLastRotation = platform.eulerAngles;
                                //                                 platRelativePosition = essentials.rigidbody.transform.position - platform.transform.position;
                                //                                 ledgePos = ledgeHit.point;
                                //                                 platformVelocity = Vector3.zero;
                                //                             }
                            }
                        }
                    }
                    else if (onLedge)
                    {
                        if (!hit)
                        {
                            float checkAngle = Vector3.Angle(essentials.rigidbody.transform.up, ledgeHit.normal);
                            if (checkAngle <= standAngle)
                            {

                                // Reset values
                                onLedge = true;
                                state.SetActionState("ledge", 0.04f);
                                hit = true;
                                // if (ledgeHit.transform.gameObject.tag == "Platform")
                                // {
                                //     forcePos = ledgeHit.point;
                                //     applyForce = true;

                                //     if (ledgeHit.transform != groundObject)
                                //     {
                                //         groundObject = ledgeHit.transform;
                                //         Rigidbody r = ledgeHit.rigidbody;
                                //         if (r != null)
                                //         {
                                //             groundBody = r;
                                //         }
                                //         else
                                //         {
                                //             groundBody = null;
                                //         }
                                //     }
                                // }
                            }
                        }
                    }
                }
            }
        }

        if (!hit)
        {
            onLedge = false;
        }
    }

    public void LedgeJump()
    {
        // Get off ledge

        bool wantJump = true;
        Vector3 inputDir = Vector3.zero;

        // Ledge Up
        // if (wantJump && inputDir.y >= 0f && ledgeTime >= minLedgeTime)
        // {
        //     onLedge = false;
        //     ledgeTime = 0f;
        //     isLedgeUpJumping = true;
        //     state.SetActionState("ledgejump", 0.4f);
        //     essentials.rigidbody.AddForce(essentials.rigidbody.transform.up * ledgeUpForce);
        // }

        // Ledge down
        // else if (wantJump && inputDir.y <= 0f && ledgeTime >= minLedgeTime)
        // {
        onLedge = false;
        ledgeTime = 0f;
        essentials.rigidbody.AddForce(-essentials.rigidbody.transform.forward * pushAwayForce, ForceMode.Impulse);
        // }


        // Jumping
        // if (isLedgeUpJumping)
        // {
        //     ledgeUpTime += Time.deltaTime;
        //     if (ledgeUpTime >= ledgeUpDuration)
        //     {
        //         isLedgeUpJumping = false;
        //         essentials.rigidbody.AddForce(essentials.rigidbody.transform.forward * ledgeForwardForce, ForceMode.Impulse);
        //     }
        //     else
        //     {
        //         essentials.rigidbody.linearVelocity = new Vector3(0f, ledgeUpForce * ledgeUpCurve.Evaluate(ledgeUpTime / ledgeUpDuration), 0f);
        //     }
        // }
    }
}