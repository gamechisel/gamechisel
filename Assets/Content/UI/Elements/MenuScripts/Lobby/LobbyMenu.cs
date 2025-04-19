using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;

    [Header("Selectable")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button playMenuButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button multiplayerButton;

    [Header("Menu")]
    [SerializeField] private Animator animator;

    [Header("Components")]
    [SerializeField] private TMP_Text slotText;
    [SerializeField] private TMP_Dropdown saveSlotDropdown;

    [Header("Back Buttons")]
    [SerializeField] private Button[] backButtons;

    private void Awake()
    {
        LoadSaveData();
    }

    // Add Listeners
    private void Start()
    {
        menuManager.SetMenuState("start");
        // playMenuButton.onClick.AddListener(() => OpenPlay());
        creditsButton.onClick.AddListener(() => OpenCredits());
        playButton.onClick.AddListener(() => OpenMultiplayerMenu());
        optionsButton.onClick.AddListener(() => Options());
        quitButton.onClick.AddListener(() => Quit());
        // multiplayerButton.onClick.AddListener(() => OpenMultiplayerMenu());

        for (int i = 0; i < backButtons.Length; i++)
        {
            int index = i; // Create a local variable to capture the current index
            backButtons[i].onClick.AddListener(() => BackButton());
        }

        // save Slot Selection
        // saveSlotDropdown.onValueChanged.AddListener(SelectSaveSlot);

        // UpdateUI(); // Update UI with session data
    }

    private void LoadSaveData()
    {
        // saveData = SaveGameSystem.LoadGame(1); // Load save data from slot 1 by default
    }

    private void SelectSaveSlot(int index)
    {
        // Update the selected save slot based on the dropdown menu selection
        SessionManager.Instance.sessionData.saveSlot = index + 1; // Add 1 to match the save slot index
        SessionManager.Instance.SaveUserSession();
    }

    private void ResetSaveData()
    {
        //     SaveGameSystem.ResetGame(saveSlotDropdown.value + 1); // Reset save data for the selected slot
        //     LoadSaveData(); // Reload save data
        //     UpdateUI(); // Update UI
    }

    private void UpdateUI()
    {
        // Display last saved slot
        slotText.text = "Save Slot: " + SessionManager.Instance.sessionData.saveSlot.ToString();

        // Update save slot dropdown menu
        saveSlotDropdown.ClearOptions();
        List<string> dropdownOptions = new List<string> { "Slot 1", "Slot 2", "Slot 3", "Slot 4" };
        saveSlotDropdown.AddOptions(dropdownOptions);

        // Set the initial value of the dropdown menu based on sessionData.saveSlot
        int dropdownIndex = Mathf.Clamp(SessionManager.Instance.sessionData.saveSlot - 1, 0, dropdownOptions.Count - 1);
        saveSlotDropdown.value = dropdownIndex;
    }

    // MultiplayerMenu
    private void OpenMultiplayerMenu()
    {
        UIManager.Instance.LoadMenu("multiplayer_menu");
        Close();
    }

    // _____________________________________________________________

    private void OpenCredits()
    {
        menuManager.SetMenuState("credits");
    }

    private void OpenPlay()
    {
        menuManager.SetMenuState("play");
    }

    private void Update()
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
            BackButton();
        }
    }

    public void Close()
    {
        menuManager.DestroyMenu();
    }

    private void Options()
    {
        OptionsManager.Instance.OpenOptionsMenu();
    }

    private void Quit()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            App.Instance.QuitApplication();
        }
    }

    public void OpenYouTube()
    {
        ExternalLinks.OpenYoutube();
    }

    public void OpenDiscord()
    {
        ExternalLinks.OpenDiscord();
    }

    public void OpenWebsite()
    {
        ExternalLinks.OpenWebsite();
    }

    public void OpenItch()
    {
        ExternalLinks.OpenItch();
    }

    private void BackButton()
    {
        string state = menuManager.GetState();
        if (state == "credits")
        {
            menuManager.SetMenuState("start");
        }
        else if (state == "play")
        {
            menuManager.SetMenuState("start");
        }
    }
}
