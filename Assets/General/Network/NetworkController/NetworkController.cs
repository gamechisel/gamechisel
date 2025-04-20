using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NetworkController : NetworkBehaviour
{
    public static NetworkController Instance { get; private set; }

    [Header("Info")]
    public NetworkVariable<HostData> _hostData = new NetworkVariable<HostData>(new HostData());
    public NetworkList<ClientData> _connectedClients;
    private Coroutine pingUpdateCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            //NetworkList can't be initialized at declaration time like NetworkVariable. It must be initialized in Awake instead.
            //If you do initialize at declaration, you will run into memory leak errors.
            _connectedClients = new NetworkList<ClientData>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void EnsureHostInConnectedClients()
    {
        if (!IsServer) return; // Nur der Server sollte das machen

        // Überprüfen, ob der Host bereits in der Liste ist
        bool hostExists = false;
        foreach (var client in _connectedClients)
        {
            if (client.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                hostExists = true;
                break;
            }
        }

        // Wenn der Host nicht in der Liste ist, füge ihn hinzu
        if (!hostExists)
        {
            _connectedClients.Add(new ClientData
            {
                ClientId = NetworkManager.Singleton.LocalClientId,
                Ping = 0,
                IsHost = true
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PrintData();
        }
    }

    public void PrintData()
    {
        // Print Host Data
        string hostInfo = $"Host Data: \n" +
                          $"Is Hosting: {_hostData.Value.PublicIP == NetworkConnection.Instance.publicIP}\n" +
                          $"Host IP (Local): {_hostData.Value.LocalIP}\n" +
                          $"Host IP (Public): {_hostData.Value.PublicIP}\n" +
                          $"Host Port: {_hostData.Value.Port}";
        Debug.Log(hostInfo);

        // Client Data
        Debug.Log("Connected clients: " + _connectedClients.Count);
        foreach (var client in _connectedClients)
        {
            Debug.Log("Client ID: " + client.ClientId + " Ping: " + client.Ping + " IsHost: " + client.IsHost);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) { return; }

        CreateHostData(7777);
        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        EnsureHostInConnectedClients();

        pingUpdateCoroutine = StartCoroutine(UpdatePingPeriodically());
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) { return; }

        ResetHostData();
        NetworkManager.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        // Beende die Coroutine
        if (pingUpdateCoroutine != null)
        {
            StopCoroutine(pingUpdateCoroutine);
        }
    }

    // Host Data
    public void CreateHostData(ushort port)
    {
        if (!IsServer) { return; } // Only the server can create host data.

        HostData newHostData = new HostData();

        newHostData.LocalIP = NetworkConnection.Instance.localIP;
        newHostData.PublicIP = NetworkConnection.Instance.publicIP;

        newHostData.Port = port; // The port you specify for hosting.

        // set value
        _hostData.Value = newHostData;
    }

    public void KickPlayer(ulong clientId)
    {
        if (!IsServer) { return; } // Only the server can kick players.

        // Check if the client ID is valid (you can also add additional validation here)
        if (clientId == 0)
        {
            Debug.LogError("Invalid client ID!");
            return;
        }

        // Kick the player by disconnecting them.
        NetworkManager.Singleton.DisconnectClient(clientId);

        // Optionally, you can add a message to the kicked player (in case you want to notify them).
        // SendKickMessage(clientId);

        Debug.Log($"Player with Client ID {clientId} has been kicked.");
    }

    private void ResetHostData()
    {
        // _hostData.Dispose(); // Synchronisation beenden
        _hostData = new NetworkVariable<HostData>(
            new HostData
            {
                Port = 7777,
                LocalIP = "",
                PublicIP = ""
            }
        );
    }

    // when player joins
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        bool isHostBool = NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId;

        _connectedClients.Add(
            new ClientData
            {
                ClientId = clientId,
                Ping = 0,
                IsHost = isHostBool
            }
        );
    }

    // when player leaves
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log("Client disconnected: " + clientId);
        _connectedClients.Remove(new ClientData { ClientId = clientId });
    }

    // Coroutine zur regelmäßigen Aktualisierung der Ping-Daten
    private IEnumerator UpdatePingPeriodically()
    {
        while (true)
        {
            UpdatePing();
            yield return new WaitForSeconds(5f); // Aktualisierung alle 1 Sekunde
        }
    }

    private void UpdatePing()
    {
        if (!IsServer) return;

        // RTT für jeden Client abrufen
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var clientId = client.ClientId;
            int ping = GetPingForClient(clientId);

            // Update the ping value for the existing client
            for (int i = 0; i < _connectedClients.Count; i++)
            {
                if (_connectedClients[i].ClientId == clientId)
                {
                    _connectedClients[i] = new ClientData
                    {
                        ClientId = clientId,
                        Ping = ping,
                        IsHost = _connectedClients[i].IsHost
                    };
                    break;
                }
            }
        }
    }

    // Berechnet den Ping für einen spezifischen Client
    private int GetPingForClient(ulong clientId)
    {
        // Prüfen, ob Unity Transport verwendet wird
        if (NetworkManager.Singleton.NetworkConfig.NetworkTransport is UnityTransport transport)
        {
            // RTT abrufen und in Millisekunden umrechnen
            return (int)(transport.GetCurrentRtt(clientId));
        }
        return -1; // Fallback: Ping nicht verfügbar
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnObjectServerRpc(Vector3 position, Quaternion rotation, ulong clientId, string prefabId, bool transferOwnership)
    {
        if (!IsServer) return;  // Nur der Server darf spawnen

        string searchId = prefabId;
        GameObject spawnedObject = ResourceSystem.GetGameObject(searchId);
        GameObject newObject = Instantiate(spawnedObject, position, rotation);
        NetworkObject networkObject = newObject.GetComponent<NetworkObject>();
        newObject.GetComponent<NetworkObject>().Spawn(true);

        if (transferOwnership)
        {
            networkObject.ChangeOwnership(clientId);
        }

        return;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyObjectServerRpc(NetworkObjectReference objectToDestroy)
    {
        // if (IsClient)
        // {
        //     GetComponent<NetworkSpawner>().DestroyObjectServerRpc(netObj);
        // }
        if (!IsServer) return;

        if (objectToDestroy.TryGet(out NetworkObject netObj))
        {
            netObj.Despawn(true);  // Entfernt das Objekt von allen Clients
            Destroy(netObj.gameObject);  // Löscht es endgültig auf dem Server
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void SendMessageToClientRpc(string _message)
    {
        Debug.Log(_message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendMessageToServerRpc(string _message)
    {
        Debug.Log(_message);
    }

    // returns all palyer ids
    public List<ulong> ReturnClientIdList()
    {
        List<ulong> clientIdList = new List<ulong>();

        foreach (var client in _connectedClients)
        {
            clientIdList.Add(client.ClientId);
        }

        return clientIdList;
    }

    [ClientRpc(RequireOwnership = false)]
    public void SetNewSceneClientRpc(string _newScene)
    {
        // Stop music and clear UI
        SceneLoader.Instance.sceneName = _newScene;
        AudioManager.Instance.StopMusic();
        UIManager.Instance.ClearAllMenus();
    }

    public void LoadMultiplayerScene(string id)
    {
        StartCoroutine(LoadNetworkSceneAsync(id));
    }

    // Networked Scene Loading
    private IEnumerator LoadNetworkSceneAsync(string id)
    {
        // Trigger Netcode's networked scene loading
        NetworkManager.Singleton.SceneManager.LoadScene(id, LoadSceneMode.Single);

        // Run scene switch for clients
        if (IsServer)
        {
            SceneSwitchClientRpc(id);
        }

        // Wait for networked scene loading to complete (optional check)
        while (NetworkManager.Singleton.IsServer)
        {
            yield return null; // Wait for the scene to fully load
        }

        yield break;
        // animator.SetTrigger("open");
        // yield return new WaitForSeconds(animationTime);
        // switchingScene = false;
    }
    
    [ClientRpc]
    private void SceneSwitchClientRpc(string _sceneName)
    {
       SceneLoader.Instance.SetSceneName(_sceneName);
       UIManager.Instance.ClearAllMenus();

    }
}
