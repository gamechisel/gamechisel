using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// ----------------------------------------------------------------
// Master class for the game management
public class App : MonoBehaviour
{
    public static App Instance { get; private set; }

    [Header("Master Objects")]
    public GameObject content;

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void Awake()
    {
        // Creating Instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            content.SetActive(true);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        SetUp();
    }

    void FixedUpdate()
    {

    }

    void Update()
    {

    }

    void LateUpdate()
    {

    }

    void SetUp()
    {

    }

    public void QuitApplication()
    {
        AudioManager.Instance.StopMusic();
        UIManager.Instance.ClearAllMenus();
        Debug.Log("Quit Application");
        if (Application.isEditor)
        {
            SceneLoader.Instance.LoadScene("LobbyScene");
        }
        else
        {
            Application.Quit();
        }
    }
}
