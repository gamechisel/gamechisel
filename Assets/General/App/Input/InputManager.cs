using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

// public +class PriorityInputListener
// {
//     public string id = "";
//     public int priority = 0;
// }

// -----------------------------------------------------------------------------
// Class to handle player input

public class InputManager : MonoBehaviour
{
    // Queue to store pending managers
    // [SerializeField] private List<PriorityInputListener> activeInputListeners = null;

    public static InputManager Instance { get; private set; }

    [Header("General")]
    public bool allowInput;

    [Header("Settings")]
    public bool invertHorizontal;
    public bool invertVertical;
    [SerializeField] private float sensitivity = 1f;

    [Header("PlayerInput")]
    [SerializeField] private PlayerInput playerInput;

    [Header("UI-Input")]
    private bool anyInput;
    public bool backButton;
    public bool optionsButton;

    [Header("Camera")]
    public Vector2 cam;
    public float camX;
    public float camY;
    public float camAmount;

    [Header("Basic-Input")]
    public Vector2 move;
    public float moveX;
    public float moveY;
    public float moveAmount;
    public bool jumpButton;
    public bool sprintButton;
    public bool crouchButton;
    public bool dodgeButton;
    public bool glideButton;
    public bool boardButton;
    public bool waterUpButton;
    public bool waterDownButton;

    [Header("Essentials-Input")]
    public bool interactButton;
    // public bool inventoryButton;

    [Header("Fight-Input")]
    public bool attackButton;
    public bool heavyButton;
    public bool specialAttackButton;
    public bool blockButton;
    public bool aimButton;

    [Header("Vehicle")]
    public float acceleration;
    public bool breakButton;
    public bool driftButton;

    // -----------------------------------------------------------------------------
    // Start & Awake
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
    }

    // -----------------------------------------------------------------------------
    // Update
    private void Update()
    {
        ReadCamera();
        ReadVehicle();
        ReadRegular();
        CheckUIInput();
    }

    // -----------------------------------------------------------------------------
    // Input Settings
    public void UpdateSettings(InputSettings inputSettings)
    {
        invertVertical = inputSettings.invertVertical;
        invertHorizontal = inputSettings.invertHorizontal;
        sensitivity = inputSettings.sensitivity;
    }

    // -----------------------------------------------------------------------------
    // Change input acceptance
    public void AllowInput(bool state)
    {
        allowInput = state;
    }

    // -----------------------------------------------------------------------------
    // Reset Input actions
    public void ResetBasicInput()
    {
        interactButton = false;
        backButton = false;
        cam = Vector2.zero;
        move = Vector3.zero;
        moveAmount = 0f;
        crouchButton = false;
        sprintButton = false;
        jumpButton = false;
        // attackButton = false;
        specialAttackButton = false;
        blockButton = false;
        aimButton = false;
        heavyButton = false;
        dodgeButton = false;
        waterUpButton = false;
        waterDownButton = false;
    }

    public void ResetUIInput()
    {
        anyInput = false;
        backButton = false;
        optionsButton = false;
    }

    //------------------------------------------------------------------------------------ Functions
    // UI Input
    private void CheckUIInput()
    {
        // closes open menu
        playerInput.actions["Escape"].performed += i => backButton = true;
        if (backButton && !UIManager.Instance.uiOpen)
        {
            backButton = false;
        }

        // opens options menu
        playerInput.actions["Escape"].performed += i => optionsButton = true;
        if (optionsButton)
        {
            optionsButton = false;
            if (!UIManager.Instance.uiOpen)
            {
                OptionsManager.Instance.OpenOptionsMenu();
            }
        }

        // Check input for gamepad
        if (!UISelection.Instance.isSelected)
        {
            // playerInput.actions["FindInput"].performed += i => anyInput = true;
            anyInput = false;
            if (anyInput)
            {
                anyInput = false;
                UISelection.Instance.ShouldSelected();
            }
        }
    }

    // -----------------------------------------------------------------------------
    // Camera Input
    private void ReadCamera()
    {
        // Vector2 cameraRead = Vector2.zero;
        // playerInput.actions["Camera"].performed += ctx => cameraRead = ctx.ReadValue<Vector2>();
        Vector2 cameraRead = playerInput.actions["Camera"].ReadValue<Vector2>();
        if (invertVertical) { cameraRead.y *= -1f; }
        if (invertHorizontal) { cameraRead.x *= -1f; }
        cameraRead *= sensitivity;
        cam = cameraRead;
    }

    // -----------------------------------------------------------------------------
    // Regular Input
    private void ReadRegular()
    {
        move = playerInput.actions["Move"].ReadValue<Vector2>();
        // playerInput.actions["Move"].performed += ctx => move = ctx.ReadValue<Vector2>();
        moveAmount = move.magnitude;
        playerInput.actions["Crouch"].performed += i => crouchButton = true;
        playerInput.actions["Crouch"].canceled += i => crouchButton = false;
        playerInput.actions["Sprint"].performed += i => sprintButton = true;
        playerInput.actions["Sprint"].canceled += i => sprintButton = false;
        playerInput.actions["Jump"].performed += i => jumpButton = true;
        playerInput.actions["Jump"].canceled += i => jumpButton = false;
        playerInput.actions["Heavy"].canceled += i => heavyButton = false;
        playerInput.actions["Heavy"].performed += i => heavyButton = true;
        playerInput.actions["Attack"].performed += i => attackButton = true;
        playerInput.actions["SpecialAttack"].performed += i => specialAttackButton = true;
        playerInput.actions["Block"].performed += i => blockButton = true;
        playerInput.actions["Block"].canceled += i => blockButton = false;
        playerInput.actions["Aim"].performed += i => aimButton = true;
        playerInput.actions["Aim"].canceled += i => aimButton = false;
        playerInput.actions["Crouch"].canceled += i => boardButton = false;
        playerInput.actions["Crouch"].performed += i => boardButton = true;
        playerInput.actions["Interact"].performed += i => interactButton = true;
        playerInput.actions["WaterUp"].performed += i => waterUpButton = true;
        playerInput.actions["WaterUp"].canceled += i => waterUpButton = false;
        playerInput.actions["WaterDown"].performed += i => waterDownButton = true;
        playerInput.actions["WaterDown"].canceled += i => waterDownButton = false;
        playerInput.actions["Dodge"].performed += i => dodgeButton = true;
    }

    private void ReadVehicle()
    {
        acceleration = playerInput.actions["VehicleAccelerate"].ReadValue<float>();
        playerInput.actions["VehicleBreak"].performed += i => breakButton = true;
        playerInput.actions["VehicleBreak"].canceled += i => breakButton = false;
        playerInput.actions["VehicleDrift"].performed += i => driftButton = true;
        playerInput.actions["VehicleDrift"].canceled += i => driftButton = false;
    }
}
