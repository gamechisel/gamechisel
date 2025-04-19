using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SessionManager : MonoBehaviour
{
    // Singleton instance
    public static SessionManager Instance;

    public SessionData sessionData;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            LoadUserData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to load user data
    public void LoadUserData()
    {
        sessionData = new SessionData();
        sessionData.playerId = "alpha";
        sessionData.saveSlot = 1;
        sessionData.date = System.DateTime.Now.ToString();
    }

    public void SaveUserSession()
    {
        SaveSessionSystem.SaveSession(sessionData);
    }

    public void LoadUserSession()
    {
        SaveSessionSystem.SaveSession(sessionData);
    }
}
