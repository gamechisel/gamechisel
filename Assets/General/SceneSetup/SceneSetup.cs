using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public static SceneSetup Instance { get; set; }

    [Header("Setup Variables")]
    public Transform centerPosition;

    [Header("Components")]
    public EnvironmentManager environmentManager;

    private void Awake()
    {
        Instance = this;
    }

    public void StartSetup()
    {

    }

    private void SaveSetupData()
    {

    }
}
