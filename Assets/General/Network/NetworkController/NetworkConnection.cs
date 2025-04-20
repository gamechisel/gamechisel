using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using Unity.Netcode.Transports.UTP;

[System.Serializable]
public class NetworkConnection : MonoBehaviour
{
    public static NetworkConnection Instance;

    [Header("Connection Settings")]
    public ushort defaultPort = 7777;
    public FixedString32Bytes localIP;
    public FixedString32Bytes publicIP;

    [Header("Used Connection")]
    public FixedString32Bytes sessionName = "multiplayer-test";
    public FixedString32Bytes connectionIP;
    public ushort usePort = 7777;
    public ushort hostPort = 7778;

    [Header("State")]
    public bool isHost = false;
    public bool isClient = false;

    [Header("Host-Details")]
    public bool allowClientJoining = false;
    public string requiredPassword = "secret123"; // Set host password

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

    public bool StartHost(ushort _port)
    {
        if (isHost || isClient)
        {
            return false;
        }

        localIP = GetLocalIPAddress();
        publicIP = GetPublicIPAddress();

        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = hostPort;
        // NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        // NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCheck;

        bool isHostStarted = NetworkManager.Singleton.StartHost();

        if (isHostStarted)
        {
            SpawnNetworkController();
            allowClientJoining = true;
            connectionIP = publicIP;
            usePort = _port;
            isHost = true;
            return true;
        }

        StartErrorHandling();

        return false;
    }

    private void SpawnNetworkController()
    {
        GameObject spawnController = ResourceSystem.GetSystem("network/network_controller");
        GameObject newObject = Instantiate(spawnController, Vector3.zero, Quaternion.identity);
        newObject.GetComponent<NetworkObject>().Spawn(true); // Spawnen über Netcode
    }

    public void DisableClientJoining()
    {
        allowClientJoining = false;
    }

    public bool StopHost()
    {
        // return if not possible
        if (!isHost) { return false; }

        allowClientJoining = false;
        // Shutdown host
        NetworkManager.Singleton.Shutdown();

        connectionIP = "";
        usePort = 7777;
        isHost = false;
        StopErrorHandling();
        return true;
    }

    // Check if we are already hosting a game
    private void ConnectionApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = allowClientJoining;
        response.Pending = false;
        return;

        // if (request.Payload.Length == 0)
        // {
        //     response.Approved = false;
        //     response.Pending = false;
        //     return;
        // }

        // string clientPassword = System.Text.Encoding.UTF8.GetString(request.Payload);
        // bool isPasswordCorrect = clientPassword == requiredPassword;

        // response.Approved = isPasswordCorrect && allowClientJoining;
        // response.Pending = false;
    }

    // Check if we are already hosting a game
    public bool StartClient(string _serverIp, ushort _port, string clientPassword)
    {
        if (isHost || isClient)
        {
            return false;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = _serverIp;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = _port;

        // byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(clientPassword);
        // NetworkManager.Singleton.NetworkConfig.ConnectionData = passwordBytes;

        bool isClientStarted = NetworkManager.Singleton.StartClient();

        if (isClientStarted)
        {
            connectionIP = _serverIp;
            usePort = _port;
            isClient = true;
            return true;
        }

        StartErrorHandling();

        return false;
    }

    public bool StopClient()
    {
        if (isHost) { return false; }

        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();  // Disconnect from the network

            connectionIP = "";
            usePort = 7777;
            isClient = false;
            return true;
            StopErrorHandling();
        }
        return false;
    }

    // Fix IP adress function
    public FixedString32Bytes GetLocalIPAddress()
    {
        try
        {
            string hostName = Dns.GetHostName();
            var addresses = Dns.GetHostAddresses(hostName);

            foreach (var ip in addresses)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return new FixedString32Bytes(ip.ToString());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting local IP: {e.Message}");
        }

        return new FixedString32Bytes("0.0.0.0");
    }

    public FixedString32Bytes GetPublicIPAddress()
    {
        try
        {
            using (var webClient = new WebClient())
            {
                // Query an external service to get the public IP.
                string publicIp = webClient.DownloadString("https://api.ipify.org").Trim();
                return new FixedString32Bytes(publicIp);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting public IP: {e.Message}");
        }

        return new FixedString32Bytes("0.0.0.0");
    }

    public bool IsNetworking()
    {
        if (isHost || isClient)
        {
            return true;
        }

        return false;
    }

    private void StartErrorHandling()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        // NetworkManager.Singleton.OnTransportFailure += HandleTransportFailure;
        // NetworkManager.Singleton.OnServerStopped += HandleServerStopped;
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        // Wenn WIR der Client sind
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogWarning("Verbindung zum Server verloren!");
            isClient = false;
            connectionIP = "";
            HandleDisconnectError();
        }
        else
        {
            Debug.Log($"Client {clientId} wurde getrennt (wir sind nicht gemeint).");
        }
    }

    private void HandleTransportFailure()
    {
        Debug.LogError("Netzwerktransport ist fehlgeschlagen!");
        if (NetworkManager.Singleton.IsClient)
        {
            isClient = false;
        }
        else if (NetworkManager.Singleton.IsHost)
        {
            isHost = false;
            allowClientJoining = false;
        }

        connectionIP = "";
        // Hier ebenfalls: Rücksprung oder Error-UI
    }

    private void HandleServerStopped(bool wasHost)
    {
        Debug.LogWarning("Server wurde gestoppt!");
        isHost = false;
        allowClientJoining = false;
        connectionIP = "";
    }

    private void HandleDisconnectError()
    {
        UIManager.Instance.ClearAllMenus();
        SceneLoader.Instance.LoadScene("LobbyScene");
        // Hier kannst du z. B. ein UI öffnen oder ins Hauptmenü zurückspringen
    }

    private void StopErrorHandling()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        // NetworkManager.Singleton.OnTransportFailure -= HandleTransportFailure;
        // NetworkManager.Singleton.OnServerStopped -= HandleServerStopped;
    }

}

