using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("Settings")]
    public bool locomotion;
    public bool allowInput;

    [Header("Reference")]
    private PlayerModelManager modelManager;
    private PlayerStats playerStats;
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private PlayerUI playerUI;

    public void Init(PlayerModelManager _modelManager, PlayerStats _playerStats)
    {
        modelManager = _modelManager;
        playerStats = _playerStats;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start_PlayerAction()
    {
        actionController.staminaManager = playerStats.entityState.staminaManager;
        actionController.Start_ActionController();
    }

    // Activate Player control
    public bool CanControl()
    {
        if (UIManager.Instance.uiOpen || !locomotion)
        {
            return false;
        }
        return allowInput;
    }

    // Active player physics
    public bool IsPhysics()
    {
        return locomotion;
    }

    // Update is called once per frame
    public void FU_PlayerActions()
    {
        if (locomotion)
        {
            // Update Action Controller
            UpdateActionController();

            // Update Model
            UpdateModel();

            // Update Animation
            UpdateAnimation();

            // Update UI
            UpdatePlayerUI();

            // Update camera
            // UpdateCameraY();
        }
    }

    public void SpawnPlayer()
    {
        // Set Transform
        // actionController.OverrideMode(reset)
    }

    public void UpdateActionController()
    {
        actionController.FU_PlayerActionController(IsPhysics(), CanControl());
    }

    public void UpdateModel()
    {
        modelManager.SetTransform(actionController.essentials.rigidbody.transform.position, actionController.essentials.rigidbody.transform.rotation.eulerAngles);
    }

    public void UpdateAnimation()
    {
        bool upperBody = false;
        bool action = actionController.actionState.currentState != "none";
        bool crouch = actionController.actionState.currentState == "crouch";
        bool swimming = actionController.actionState.currentState == "swim";
        bool climbing = actionController.actionState.currentState == "climbing";
        bool blocking = actionController.actionState.currentState == "blocking";
        bool ledge = actionController.actionState.currentState == "ledge";
        bool grounded = actionController.surroundings.IsGrounded();
        float horizontalVelocity = actionController.essentials.HorizontalVelocity();
        float verticalVelocity = actionController.essentials.VerticalVelocity();
        float sideVelocity = actionController.essentials.SideVelocity();
        float forwardVelocity = actionController.essentials.ForwardVelocity();
        if (crouch) { action = false; }
        Vector3 trot = actionController.actionInput.cameraDir * 3f + actionController.essentials.rigidbody.transform.position;
        modelManager.UpdateValues(action, upperBody, grounded, crouch, blocking, horizontalVelocity, verticalVelocity, sideVelocity, forwardVelocity, trot, swimming, climbing, ledge);
    }

    public void UpdatePlayerUI()
    {
        playerUI.UpdateAllUI(playerStats.entityState.healthManager.health, playerStats.entityState.healthManager.maxHealth, playerStats.entityState.staminaManager.currentStamina, playerStats.entityState.staminaManager.maxStamina, 0, 0);
    }
}