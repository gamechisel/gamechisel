using UnityEngine;

[System.Serializable]
public class PlayerActionController : MonoBehaviour
{
    [Header("Components")]
    public PlayerActionState actionState;
    public Essentials essentials;
    public Surroundings surroundings;
    public ActionSound actionSound;
    public ActionAnimation actionAnimation;
    public PlayerActionInputProvider actionInput;
    public PlayerItemManager playerItemManager;
    public ObjectMovingPhysics movingPhysics;
    public StaminaManager staminaManager;

    [Header("Actions")]
    public ActionRotation rotation;
    public AdvancedMovement movement;
    public AdvancedGrounding grounding;
    public ActionRoll roll;
    public ActionJump jump;
    public ActionSlide slide;
    public ActionAttack attack;
    public ActionBlock block;
    public ActionClimb climb;
    public ActionSwim swim;
    public ActionLedge ledge;
    public ActionDamage damage;
    public ScanForTargets targetFocus;
    private bool isAiming;

    public void OverrideState(string mode)
    {
        if (mode == "disable")
        {
            // surroundings.Reset();
            // actionState.ClearState();
            // actionInput.Reset_ActionInput()
        }
        else if (mode == "reset")
        {
            // surroundings.Reset();
            // actionState.ClearState();
            // actionInput.Reset_ActionInput()
        }
    }

    public bool CheckStamina(float amount)
    {
        if (staminaManager.currentStamina >= amount)
        {
            staminaManager.currentStamina -= amount;
            return true;
        }
        return false;
    }

    // Start ActionController
    public void Start_ActionController()
    {
        // Part 1 : Init Base Components
        // ___________________________________________________________

        // use character essential values to scan surroundings
        surroundings.Init(essentials);

        // use character values to affect base moving physics, weight and rigidbody
        movingPhysics.Init(essentials);

        // Part 2 : Init Actions
        // ___________________________________________________________

        // Rotation for the character
        rotation.Init(essentials.rigidbody); // use only rigidbody for reusability

        // Movement
        movement.Init(essentials, actionSound, surroundings, staminaManager);

        // Grounding
        grounding.Init(essentials);

        // Roll
        roll.Init(essentials, actionSound, actionAnimation, surroundings, actionState, rotation);

        // Jump
        jump.Init(essentials, actionAnimation, actionSound, actionState, surroundings, staminaManager);

        // Slide
        slide.Init(essentials, actionSound, actionAnimation, actionState);

        // Attack
        attack.Init(actionSound, actionAnimation, actionState, essentials, this);

        // Block
        block.Init(essentials, actionSound, actionAnimation, actionState);

        // Climb
        climb.Init(essentials, actionAnimation, actionState);

        // Swim
        swim.Init(essentials, actionAnimation, actionSound);

        // Ledge
        ledge.Init(essentials, actionSound, actionAnimation, actionState, surroundings, actionInput);

        // Damage
        damage.Init(essentials, actionAnimation, actionSound, actionState);
    }

    // Update is called once per frame
    public void FU_PlayerActionController(bool isPhysics, bool canControl)
    {
        if (!isPhysics) return;

        // is affected by platform moving -> needs to be assigned first or doesnt apply movement changes
        // when not platform: no changes
        // ledge, hanging, climbing, swimming, etc.
        PlatformAffected();

        // Scans the surroundings of character
        surroundings.FU_Surroundings();

        if (canControl)
        {
            // Read input
            actionInput.FU_ActionInput();
        }
        else
        {
            actionInput.ResetOldInput();
        }

        // Update current action State
        actionState.FU_State();

        // Surroundings State Update
        // Breaks up states
        SurroundingsStateUpdate();

        // Update generell
        UpdateAction(actionState.currentState);

        // Stamina
        staminaManager.RegenerateStamina();

        // Update Collider
        UpdateCollider();

    }

    private void UpdateAction(string _state)
    {
        // CHAPTER  2 : Default Updates
        // ___________________________________________________________

        // Default Updates
        bool disableGravity = false;
        bool disableDrag = false;
        bool disableGroundFriction = false;

        // Air Ability
        jump.UpdateJump();

        if (Input.GetMouseButtonDown(1)) isAiming = true;
        if (Input.GetMouseButtonUp(1)) isAiming = false;

        // if (isAiming && targetFocus.enemiesInRange[targetFocus.currentTargetIndex])
        // {
        // }
        // rotation.RotateToVelocity();

        // CHAPTER  3 : Custom Game Action Logic
        // ___________________________________________________________
        switch (_state)
        {
            case "none":

                // Rotation
                rotation.RotateToVelocity();

                // TryCrouch
                TryCrouch();

                HandleGroundingAlign();

                // Movement
                movement.FU_Movement(actionInput.moveDir, actionInput.wantCrouch, actionInput.wantSprint, false);
                movement.Move(actionInput.moveDir, actionInput.wantCrouch, actionInput.wantSprint, false);

                // Jump
                TryJump();

                // Roll
                TryRoll();

                // TryAttack
                TryAttack();

                // TryBlock
                TryBlock();

                // ledge
                // if (essentials.rigidbody.linearVelocity.y < 0.1f && !surroundings.IsGrounded() && !surroundings.IsSlope())
                // {
                //     ledge.TryLedge();
                // }

                // climb
                // climb.TryClimb();

                if (actionInput.wantHeavyAttack)
                {
                    attack.TryHeavyAttack(actionInput.wantHeavyAttack);
                }


                break;

            case "swim":
                disableGravity = true;

                if (surroundings.foundWaterSurface && !actionInput.wantSwimDown)
                {
                    swim.WaterFloating(surroundings.waterLevel);
                }

                if (surroundings.foundWaterSurface)
                {
                    actionInput.wantSwimUp = false;
                }

                swim.WaterMovement(actionInput.moveDir, actionInput.wantSwimUp, actionInput.wantSwimDown);

                movingPhysics.WaterDrag();
                rotation.RotateToVelocity();

                // GroundAlign
                if (surroundings.IsGrounded())
                {
                    grounding.GroundAlign(essentials.rideHeight, surroundings.hitDistance, 1f, true, false, surroundings.groundNormal);
                }

                break;

            case "climbing":
                disableGravity = true;

                climb.ClimbMovement(actionInput.moveDirVertical);

                break;

            case "crouch":

                // Crouch
                TryCrouch();

                // Jump
                TryCrouchJump();

                // Rotation
                rotation.RotateToVelocity();

                // GroundAlign
                HandleGroundingAlign();

                // Movement
                movement.FU_Movement(actionInput.moveDir, actionInput.wantCrouch, actionInput.wantSprint, false);
                movement.Move(actionInput.moveDir, actionInput.wantCrouch, actionInput.wantSprint, false);

                // Jump
                TryCrouchJump();

                // Roll
                TryRoll();


                break;

            case "sprint":

                // Sprint

                // Rotation
                rotation.RotateToVelocity();

                // Roll
                TryRoll();

                break;

            case "jump":
                break;

            case "rolljump":
                break;

            case "heightjump":
                break;

            case "crouchjump":
                break;



            case "roll":
                HandleGroundingAlign();

                // TODO:
                // Move last frames
                // Friction to ground only last seconds
                // HandleFriction();

                // Apply little Slope Force to slope direction

                // Rotation
                rotation.RotateToVelocity();

                // Friction disable
                disableGroundFriction = true;

                // Force Crouch
                if (actionState.actionTime <= 0.02f)
                {
                    // Crouch
                    if (
                    surroundings.isSomethingAboveHead && surroundings.IsGrounded() && !surroundings.IsSlope() ||
                    surroundings.IsGrounded() && !surroundings.IsSlope() && actionInput.wantCrouch)
                    {
                        actionState.SetActionState("crouch", 0.04f);
                    }
                }

                if (actionState.actionTime <= 0.4f)
                {
                    // Movement
                    movement.FU_Movement(actionInput.moveDir, actionInput.wantCrouch, actionInput.wantSprint, false);
                    movement.Move(actionInput.moveDir, actionInput.wantCrouch, actionInput.wantSprint, false);
                }

                // RollJump
                TryRollJump();

                break;

            case "fly":
                disableDrag = true;
                disableGravity = true;
                break;

            case "attack":

                break;

            case "light_attack1":

                break;

            case "light_attack2":

                break;

            case "light_attack3":

                break;

            case "heavyloading":

                break;

            case "uppunch":
                break;

            case "dashpunch":
                break;

            case "block":

                // Roll -> break
                TryRoll();

                break;

            case "aim":

                // Roll -> break
                TryRoll();

                break;

            case "ledge":
                disableGravity = true;

                ledge.TryLedge();
                essentials.rigidbody.linearVelocity = Vector3.zero;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ledge.LedgeJump();
                }

                disableGroundFriction = true;
                disableDrag = true;
                disableGravity = true;

                break;

            case "ledgejump":
                break;

            default:

                HandleGroundingAlign();

                // Rotation
                rotation.RotateToVelocity();

                // Roll
                TryRoll();

                break;
        }

        // CHAPTER  4 : Default Updates
        // ___________________________________________________________

        // Apply GroundFiction
        if (!disableGroundFriction)
        {
            // HandleFriction();
        }

        // Apply Gravity
        if (!disableGravity)
        {
            HandleGravity();
        }

        // Apply Drag
        if (!disableDrag)
        {
            HandleDrag();
        }
    }

    // Basic Methods
    // ___________________________________________________________________

    private void SurroundingsStateUpdate()
    {
        // Update Surroundings
        // break up states
        if (surroundings.inWater)
        {
            actionState.SetActionState("swim", 0.04f);
        }

        // Force Crouch
        if (!surroundings.inWater)
        {
            if (!surroundings.IsSlope() && surroundings.IsGrounded() && surroundings.isSomethingAboveHead)
            {
                if (!actionState.currentState.Equals("roll"))
                {
                    actionState.SetActionState("crouch", 0.04f);
                }
            }
        }
    }

    private void PlatformAffected()
    {
        // Apply Platform Data
        if (surroundings.platformData.platformObject != null)
        {
            // Only if on Ground and not in water
            if (surroundings.IsGrounded() && !surroundings.inWater)
            {
                // Check against action states
                if (!actionState.currentState.Equals("climbing") && !actionState.currentState.Equals("ledge") && !actionState.currentState.Equals("ledgejump") && !actionState.currentState.Equals("swim"))
                {
                    // Apply platform change
                    surroundings.platformData.FU_Platform(essentials.rigidbody.transform);
                }
            }
        }
    }

    private void UpdateCollider()
    {
        if (actionState.currentState == "crouch" || actionState.currentState == "roll")
        {
            actionState.ActivateCollider("crouch");
        }
        else
        {
            actionState.ActivateCollider("regular");
        }
    }

    // Basic Physics
    // ___________________________________________________________________

    private void HandleGroundingAlign()
    {
        // Standing Force (Both Directions)
        if (surroundings.IsGrounded() && !surroundings.IsSlope())
        {
            grounding.GroundAlign(essentials.rideHeight, surroundings.hitDistance, 1f, false, false, surroundings.groundNormal);
        }
    }

    private void HandleDrag()
    {
        // Handle Drag
        if (surroundings.inWater)
        {
            movingPhysics.WaterDrag();
        }
        else
        {
            movingPhysics.AirDrag();
        }
    }

    private void HandleGravity()
    {
        // surroundings
        if (!surroundings.inWater)
        {
            // Handle Gravity
            if (!surroundings.IsGrounded() || surroundings.IsSlope())
            {
                movingPhysics.Gravity();
            }
        }
    }

    private void HandleFriction()
    {
        if (surroundings.IsGrounded() || surroundings.IsSlope())
        {
            movingPhysics.GroundFriction();
        }
    }

    // Action Performing
    // ___________________________________________________________________

    private void DisableAirJump()
    {
        jump.SetAirJump(false);
    }

    private void TryJump()
    {
        // Jump Ability
        bool wantJumpBool = actionInput.wantJumpTime > 0f;

        // Jump
        if (wantJumpBool)
        {
            bool newWantJump = jump.TryJump();
            if (newWantJump)
            {
                actionInput.wantJumpTime = 0f;
            }
        }
    }

    private void TryCrouchJump()
    {
        // Jump
        if (actionInput.wantJump)
        {
            if (jump.TryCrouchJump())
            {
                actionInput.wantJump = false;
            }
        }
    }

    private void TryRollJump()
    {
        bool wantJumpBool = actionInput.wantJumpTime > 0f;

        if (wantJumpBool)
        {
            if (!surroundings.isSomethingAboveHead && surroundings.IsGrounded() && !surroundings.IsSlope() && actionState.actionTime <= 0.04f)
            {
                if (actionInput.wantJumpTime > 0f)
                {
                    bool newWantJump = jump.TryRollJump();
                    if (newWantJump)
                    {
                        actionInput.wantJumpTime = 0f;
                    }
                }
            }

        }
    }

    private void TryCrouch()
    {
        // Crouch
        if (actionInput.wantCrouch && !surroundings.IsSlope() && surroundings.IsGrounded())
        {
            actionState.SetActionState("crouch", 0.04f);
        }
    }

    private void TrySprint()
    {
        // Sprint
        if (actionInput.wantSprint && !surroundings.IsSlope() && surroundings.IsGrounded())
        {
            actionState.SetActionState("sprint", 0.04f);
        }
    }

    private void TryRoll()
    {
        if (actionInput.wantRoll)
        {
            // Check Roll Requirements
            if (actionState.currentState == "none")
            {
                if (surroundings.IsGrounded() && !surroundings.IsSlope())
                {
                    Vector3 velDir = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
                    Vector3 rollDir = actionInput.moveDir;
                    if (rollDir == Vector3.zero)
                    {
                        rollDir = velDir.normalized;
                    }
                    roll.WantRoll(rollDir, velDir);
                }
                else
                {
                    Vector3 velDir = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
                    Vector3 rollDir = actionInput.moveDir;
                    if (rollDir == Vector3.zero)
                    {
                        rollDir = velDir.normalized;
                    }
                    roll.WantRoll(rollDir, velDir);
                }
            }
        }
    }

    private void TryAttack()
    {
        // Simple
        if (actionInput.wantAttack)
        {
            actionInput.wantAttack = false;
            if (actionState.currentState == "none")
            {
                attack.LightAttack(essentials.rigidbody.transform.forward);
            }
        }

        // Heavy
        if (actionInput.wantHeavyAttack)
        {
            actionInput.wantHeavyAttack = false;
            if (actionState.currentState == "none")
            {
                // attack.HeavyAttack(essentials.rigidbody.transform.forward);
                // TryHeavyAttack
                // charge attack
                // attack.UpdateHeavyCharging(actionInput.wantHeavyAttack);
            }
        }
    }

    private void TryBlock()
    {
        if (actionInput.wantBlock)
        {
            block.StartBlock();
        }
    }

    private void TryClimb()
    {

    }

    private void TryLedge()
    {

    }

    public void UseSwordItem(bool _state)
    {
        playerItemManager.UseSword(_state);
    }
}
