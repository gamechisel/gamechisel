using UnityEngine;
using System.IO;
using System;

public static class SaveSessionSystem
{
    private const string sessionFileName = "session.txt";

    public static void SaveSession(SessionData sessionData)
    {
        string currentDate = DateTime.Now.ToString();
        sessionData.date = currentDate;

        string dir = Application.persistentDataPath;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string json = JsonUtility.ToJson(sessionData);
        File.WriteAllText(Path.Combine(dir, sessionFileName), json);
        Debug.Log("Session Saved: " + Path.Combine(dir, sessionFileName));
    }

    public static SessionData LoadSession()
    {
        string fullPathSaveFile = Path.Combine(Application.persistentDataPath, sessionFileName);
        SessionData sessionData = new SessionData();
        if (File.Exists(fullPathSaveFile))
        {
            string json = File.ReadAllText(fullPathSaveFile);
            sessionData = JsonUtility.FromJson<SessionData>(json);
            Debug.Log("Session Loaded");
        }
        return sessionData;
    }

    public static void ResetGame()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, sessionFileName));
    }
}
