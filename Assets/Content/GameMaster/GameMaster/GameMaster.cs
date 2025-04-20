using UnityEngine;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance;

    [Header("Reference")]
    public TimelineManager timelineManager;
    public GameData gameData;

    [Header("Temporary")]
    public bool startGame = false;

    [Header("State")]
    public bool isRunning;
    public bool isCutscene;
    public bool isPhysics;
    public bool isPausing;
    public bool isTransitioning;

    [Header("Game Details")]
    // [SerializeField] public SavePointManager savePointManager;
    public string worldID;

    [Header("Environment Details")]
    public int time;
    public string weather;

    [Header("Quest")]
    public string currentEvent;
    public string currentQuest;

    [Header("Audio Details")]
    public bool playAudio;
    public bool readInput;

    // List of players
    [Header("Players")]
    public int playerCount;
    public List<ulong> playerIds = new List<ulong>();
    public List<PlayerState> playerStates = new List<PlayerState>();

    [System.Serializable]
    public class PlayerState
    {
        public ulong playerId;
        public bool isAlive;

        public PlayerState(ulong id)
        {
            playerId = id;
            isAlive = true;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        AudioManager.Instance.StopMusic();
        UIManager.Instance.ClearAllMenus();
    }

    private void Start()
    {
        // Add all player ids to list
        if (NetworkConnection.Instance.IsNetworking())
        {
            playerIds = NetworkController.Instance.ReturnClientIdList();
            playerCount = playerIds.Count;
            foreach (ulong id in playerIds)
            {
                playerStates.Add(new PlayerState(id));
            }
        }
    }

    public void SetPlayerState(ulong playerId, bool isAlive)
    {
        PlayerState player = playerStates.Find(p => p.playerId == playerId);
        if (player != null)
        {
            player.isAlive = isAlive;
            Debug.Log($"Player {playerId} is now {(isAlive ? "Alive" : "Dead")}");
        }
        else
        {
            Debug.LogError($"Player with ID {playerId} not found!");
        }

        UpdatePlayersDead();
    }

    // Get player state (alive or dead)
    public bool GetPlayerState(ulong playerId)
    {
        PlayerState player = playerStates.Find(p => p.playerId == playerId);
        if (player != null)
        {
            return player.isAlive;
        }
        else
        {
            Debug.LogError($"Player with ID {playerId} not found!");
            return false; // If player is not found, return false (dead)
        }
    }

    private void UpdatePlayersDead()
    {
        // Check if all players are dead
        foreach (PlayerState player in playerStates)
        {
            if (player.isAlive)
            {
                return; // If any player is alive, return
            }
        }

        // All dead
        // load latest savepoint
    }

    private void RevivewPlayers()
    {
        // Revive all players
        foreach (PlayerState player in playerStates)
        {
            player.isAlive = true;
        }
    }

    private void FixedUpdate()
    {
        if(startGame || Input.GetKeyDown(KeyCode.P))
        {
            startGame = false;
            StartGame();
            Debug.Log("Start Game");
        }
    }

    private void StartGame()
    {
        gameData.Load();
        SpawnPlayers();
        TimelineManager.Instance.PlayCutscene("cutscene/freeworld_cutscene");
    }

    private void SpawnPlayers()
    {
        // Spawn Players
        // Vector3 spawnPos = gameData.data.savePoint.position;
        // Vector3 spawnRot = gameData.data.savePoint.rotation;
        Vector3 spawnPos = Vector3.zero;
        Vector3 spawnRot = Vector3.zero;
        Quaternion spawnRotation = Quaternion.Euler(spawnRot);

        Debug.Log("spawn players");
        foreach (ulong player in playerIds)
        {
            // add distance
            Debug.Log("spawn player " + player.ToString());
            spawnPos.x += 0.5f;
            NetworkController.Instance.SpawnObjectServerRpc(spawnPos, spawnRotation, player, "game/player/player_manager", true);
        }
    }
}

