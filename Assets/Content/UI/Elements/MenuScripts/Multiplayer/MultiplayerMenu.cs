using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private NetcodeUI netcodeUI;

    [Header("Selectable")]
    [SerializeField] private Button[] backButtons;
    [SerializeField] private Animator animator;

    [Header("Buttons")]
    [SerializeField] private Button hostTab;
    [SerializeField] private Button clientTab;

    private void Start()
    {
        // Buttons
        hostTab.onClick.AddListener(() => menuManager.SetMenuState("host"));
        clientTab.onClick.AddListener(() => menuManager.SetMenuState("client"));

        // Back Button
        foreach (Button backButton in backButtons)
        {
            backButton.onClick.AddListener(() => Back());
        }

        menuManager.SetMenuState("host");
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

    public void Back()
    {
        string state = menuManager.GetState();
        if (state == "start")
        {
            Close();
        }
        else if (state == "host" || state == "client")
        {
            menuManager.SetMenuState("start");
            netcodeUI.DisconnectPlayer();
        }
    }

    private void DisconncetAndClose()
    {
        netcodeUI.DisconnectPlayer();
        Close();
    }

    public void Close()
    {
        UIManager.Instance.LoadMenu("lobby_menu");
        menuManager.DestroyMenu();
    }
}
