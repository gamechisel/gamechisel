using Unity.Netcode;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Identification")]
    public string playerName;
    public ulong playerId;
    public bool isOwner = true;

    [Header("Player Data")]
    private PlayerSaveData playerSaveData;

    [Header("References")]
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public CameraManager cameraManager;
    [SerializeField] public GameObject playerUIObject;
    [SerializeField] public GameObject playerCameraObject;
    private Transform cameraTransform;

    [Header("Components Reference")]
    public PlayerModelManager modelManager;
    public PlayerAction playerAction;
    public PlayerStats playerStats;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        cameraTransform = CameraManager.Instance.cameraTransform;
        playerAction.Init(modelManager, playerStats);

    }

    private void Start()
    {
        if (NetworkConnection.Instance.IsNetworking())
        {
            playerId = this.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            isOwner = playerId == NetworkManager.Singleton.LocalClientId;
        }
        else
        {
            isOwner = true;
        }
        
        if (!isOwner) 
        { 
            return;    
        }
        else
        {
            playerUIObject.SetActive(true);
            playerCameraObject.SetActive(true);
            playerAction.Start_PlayerAction();
        }
    }

    public void FixedUpdate()
    {
        if (!isOwner) { return; }
        playerAction.FU_PlayerActions();
    }

    public void SetPlayerTransform(Vector3 position, Vector3 eulerRotation)
    {
        modelManager.SetTransform(position, eulerRotation);
    }

    public void SetupPlayer(PlayerSaveData data)
    {

    }

    public PlayerSaveData GetPlayerSaveData()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.playerIdentifier = "";
        data.health = 100f;
        data.stamina = 100f;
        return data;
    }
}

// all actions need to be triggered by server and updated on client
// may result in cheating but faster updates and integration
