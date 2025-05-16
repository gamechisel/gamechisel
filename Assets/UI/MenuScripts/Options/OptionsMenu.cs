using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class OptionsMenu : MonoBehaviour
{
    [Header("Selectable")]
    public MenuManager menuManager;
    [SerializeField] private Button[] backButtons;
    [SerializeField] private Animator animator;

    [Header("Buttons")]
    [SerializeField] private Button menuButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button videoButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button controlsButton;

    private void Start()
    {
        // Buttons
        audioButton.onClick.AddListener(() => menuManager.SetMenuState("audio"));
        videoButton.onClick.AddListener(() => menuManager.SetMenuState("video"));
        graphicsButton.onClick.AddListener(() => menuManager.SetMenuState("graphics"));
        controlsButton.onClick.AddListener(() => menuManager.SetMenuState("controls"));
        menuButton.onClick.AddListener(() => LoadMain());

        // Back Button
        foreach (Button backButton in backButtons)
        {
            backButton.onClick.AddListener(() => Back());
        }

        menuManager.SetMenuState("start");
    }

    public void Update()
    {
        if (menuManager.isActiveMenu)
        {
            ReadInput();
        }
    }

    private void ReadInput()
    {
        if (InputManager.Instance.backButton)
        {
            InputManager.Instance.backButton = false;
            Back();
        }
    }

    private void Back()
    {
        string state = menuManager.GetState();
        if (state == "start")
        {
            Close();
        }
        else if (state == "audio" || state == "video" || state == "graphics" || state == "controls")
        {
            menuManager.SetMenuState("start");
        }
    }

    private void SaveApySettings()
    {

    }

    public void Close()
    {
        menuManager.DestroyMenu();
    }

    private void LoadMain()
    {
        StartCoroutine(LoadMainEnum());
    }

    private IEnumerator LoadMainEnum()
    {
        // Check if the current scene is not "LobbyScene"
        if (SceneLoader.Instance.sceneName != "LobbyScene")
        {
            // Check if the player is hosting the server or is a client
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    // Shut down the server
                    NetworkConnection.Instance.StopHost();
                }
                // Check if the player is a client
                else if (NetworkManager.Singleton.IsClient)
                {
                    // Shut down the server
                    NetworkConnection.Instance.StopClient();
                }
            }

            // Now that the server or client has been shut down, load the "LobbyScene"
            SceneLoader.Instance.LoadScene("LobbyScene");
            Close();
        }
        else
        {
            // Close the UI if already in "LobbyScene"
            Close();
        }

        yield return null;
    }

}
