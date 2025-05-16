using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    [Header("General")]
    public string playerIdentifier;

    [Header("State")]
    public float health;
    public float stamina;
}