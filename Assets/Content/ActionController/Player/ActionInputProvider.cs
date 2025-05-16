using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerActionInputProvider
{
    [Header("Action Input")]
    public bool wantJump = false;
    public bool wantJumpDebug = false;
    public float wantJumpTime = 0f;

    public bool wantRoll = false;
    public bool wantSprint = false;
    public bool wantCrouch = false;
    public bool wantBlock = false;
    public bool wantAttack = false;
    public bool wantHeavyAttack = false;
    public bool wantMove = false;
    public Vector3 moveDir = Vector3.zero;
    public Vector3 moveDirVertical = Vector3.zero;
    public Vector3 cameraDir = Vector3.zero;
    public Vector3 projectedDir = Vector3.zero;
    public bool wantStrafe = false;
    public bool wantShoot = false;
    public bool wantInteract = false;
    public bool wantSwimUp = false;
    public bool wantSwimDown = false;
    public bool specialButton = false;
    public bool wantDash = false;
    public bool waveAttack = false;
    public bool wantThrow = false;

    public void FU_ActionInput()
    {
        ResetOldInput();
        ReadNewInput();
    }

    public void ResetOldInput()
    {
        wantJump = false;
        wantJumpDebug = false;
        wantRoll = false;
        wantAttack = false;
        wantHeavyAttack = false;
        wantSprint = false;
        wantCrouch = false;
        wantMove = false;
        wantShoot = false;
        wantStrafe = false;

        moveDir = Vector3.zero;
        moveDirVertical = Vector3.zero;
        cameraDir = Vector3.zero;
        projectedDir = Vector3.zero;
    }

    public void ReadNewInput()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        // Want Jump Time
        if (!InputManager.Instance.jumpButton)
        {
            wantJumpDebug = false;
        }

        if (wantJumpTime > 0f)
        {
            wantJumpTime -= Time.deltaTime;
        }

        if (InputManager.Instance.jumpButton && !wantJumpDebug)
        {
            wantJumpTime = 0.15f;
            wantJump = true;
            wantJumpDebug = true;
        }

        // Dodge Button
        if (InputManager.Instance.dodgeButton)
        {
            InputManager.Instance.dodgeButton = false;
            wantRoll = true;
        }

        if (InputManager.Instance.attackButton)
        {
            InputManager.Instance.attackButton = false;
            wantAttack = true;
        }

        wantBlock = InputManager.Instance.blockButton;

        wantHeavyAttack = InputManager.Instance.heavyButton;

        wantStrafe = InputManager.Instance.blockButton;
        wantSprint = InputManager.Instance.sprintButton;
        wantCrouch = InputManager.Instance.crouchButton;

        wantSwimUp = InputManager.Instance.waterUpButton;
        wantSwimDown = InputManager.Instance.waterDownButton;

        moveDir = Vector3.zero;
        Transform camera = CameraManager.Instance.cameraTransform;
        if (camera != null)
        {
            cameraDir = camera.forward;
            moveDir = InputUtils.MoveDirection(camera, InputManager.Instance.move, InputManager.Instance.moveAmount);
        }
        moveDirVertical = InputManager.Instance.move * InputManager.Instance.moveAmount;
        projectedDir = InputUtils.LookDirection(camera, true);
    }
}