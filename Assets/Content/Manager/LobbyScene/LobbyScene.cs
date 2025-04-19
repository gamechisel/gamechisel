using UnityEngine;

// starts the main menu scene

// ----------------------------------------------------------------
// Class manages the main scene
public class LobbyManager : MonoBehaviour
{
    // Singleton instance
    public static LobbyManager Instance { get; private set; }

    // ----------------------------------------------------------------
    // Create instance
    private void Awake()
    {
        Instance = this;
    }

    // ----------------------------------------------------------------
    // Set up main scene
    private void Start()
    {
        OpenLobbyMenu();
        MenuMusic();
    }

    // ----------------------------------------------------------------
    // Play Music
    private void MenuMusic()
    {
        AudioManager.Instance.PlayMusic("wowie", true, true);
    }

    // ----------------------------------------------------------------
    // Main Menu
    private void OpenLobbyMenu()
    {
        UIManager.Instance.LoadMenu("lobby_menu");
    }
}
