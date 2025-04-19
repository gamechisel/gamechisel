using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    [Header("General")]
    public string playerIdentifier;

    [Header("Position")]
    public Vector3 savePosition;
    public Vector3 saveRotation;

    [Header("State")]
    public float health;
    public float stamina;
    // public PlayerInventorySystem inventory;
}