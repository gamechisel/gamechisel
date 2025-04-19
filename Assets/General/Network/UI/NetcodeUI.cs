using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

[System.Serializable]
public class NetcodeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MenuManager menuManager;

    [Header("UI Elements")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    [Header("Host Settings")]
    [SerializeField] private Button disconnectButton;
    [SerializeField] private TMP_Text publicIpAddressText;
    [SerializeField] private TMP_Text localIpAddressText;
    [SerializeField] private Button startButton;


    [Header("Client Settings")]
    [SerializeField] private Button disconnectButton2;
    [SerializeField] private TMP_InputField ipAddressField;
    [SerializeField] private TMP_InputField portField;


    [Header("Base Settings")]
    [SerializeField] private TMP_Text playerListText;


    private void Awake()
    {
        // Display the local and public IP addresses on startup
        localIpAddressText.text = "Local IP: " + NetworkConnection.Instance.GetLocalIPAddress().ToString();
        publicIpAddressText.text = "Public IP: " + NetworkConnection.Instance.GetPublicIPAddress().ToString();

        hostButton.onClick.AddListener(() =>
        {
            // Set the port before starting the host
            ushort port = GetPort();
            if (NetworkConnection.Instance.StartHost(port))
            {
            }
        });

        clientButton.onClick.AddListener(() =>
        {
            // Set the IP address and port before starting the client
            string ipAddress = ipAddressField.text;
            ushort port = GetPort();

            if (NetworkConnection.Instance.StartClient(ipAddress, port, "secret123"))
            {
            }
        });

        disconnectButton.onClick.AddListener(() =>
            {
                DisconnectPlayer();
            });

        disconnectButton2.onClick.AddListener(() =>
        {
            DisconnectPlayer();
        });

        startButton.onClick.AddListener(() => StartButton());
    }

    private void Update()
    {
        // Ensure the disconnect button and state text updates dynamically
        UpdateDisconnectButtonState();
        UpdatePlayerList();
    }

    public void DisconnectPlayer()
    {
        if (NetworkConnection.Instance.isHost)
        {
            NetworkConnection.Instance.StopHost();
        }
        else if (NetworkConnection.Instance.isClient)
        {
            NetworkConnection.Instance.StopClient();
        }
    }

    private void UpdateDisconnectButtonState()
    {
        disconnectButton.interactable = NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient;
    }

    private void UpdatePlayerList()
    {
        if (NetworkController.Instance != null)
        {
            playerListText.text = "Connected Players:\n";
            NetworkController activeController = NetworkController.Instance;
            foreach (var clientData in activeController._connectedClients)
            {
                playerListText.text += clientData.ClientId.ToString() + " " + clientData.IsHost.ToString() + " " + clientData.Ping.ToString() + "\n";
            }
        }
        else
        {
            playerListText.text = "null";
        }
    }

    private ushort GetPort()
    {
        ushort port;
        if (!ushort.TryParse(portField.text, out port))
        {
            port = 7777; // Default port if input is invalid
        }
        return port;
    }

    private void StartButton()
    {
        NetworkConnection.Instance.DisableClientJoining();
        UIManager.Instance.ClearAllMenus();
        string _sceneName = "FreeWorld";
        NetworkController.Instance.LoadMultiplayerScene(_sceneName);
        SceneLoader.Instance.sceneName = _sceneName;
    }

    private void Close()
    {
        // DisconnectPlayer();
        // menuManager.DestroyMenu();
    }
}