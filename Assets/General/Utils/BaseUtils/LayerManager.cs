using UnityEngine;

public static class LayerManager
{
    // Einzelne LayerMasks als LayerMask Instanzen (direkt verwendbar)
    public static LayerMask GroundLayer => LayerMask.GetMask("Ground_Collision");           // Layer für den Boden
    public static LayerMask HitboxLayer => LayerMask.GetMask("Hitbox");           // Layer für die Hitboxen von Entitäten
    public static LayerMask WaterLayer => LayerMask.GetMask("Water");             // Layer für Wasserflächen
    public static LayerMask EntityCollisionLayer => LayerMask.GetMask("Entity_Collision");  // Layer für Entitäten, die Kollisionen haben
    public static LayerMask InteractableLayer => LayerMask.GetMask("Interactable"); // Layer für Interaktionsobjekte
    public static LayerMask CollectableLayer => LayerMask.GetMask("Collectable"); // Layer für Sammlerstücke
    public static LayerMask ObjectCollisionLayer => LayerMask.GetMask("Object_Collision"); // Layer für Objektkollisionen
    public static LayerMask EditorLayer => LayerMask.GetMask("Editor");           // Layer für Editor-Objekte
    public static LayerMask TargetLayer => LayerMask.GetMask("Hitbox");           // Layer für Editor-Objekte

    // Kombinierte Layer (verwendet LayerMask und kombiniert verschiedene Layer)
    public static LayerMask FlyAboveLayer =>
        LayerMask.GetMask("Ground_Collision", "Water");  // Kombinierter Layer für "über dem Boden fliegende" Entitäten

    public static LayerMask ObstaclesLayer =>
        LayerMask.GetMask("Ground_Collision", "Water", "Entity_Collision", "Object_Collision");  // Kombinierter Layer für "über dem Boden fliegende" Entitäten

    public static LayerMask AllEntityLayers =>
        LayerMask.GetMask("Hitbox", "Hitbox", "Interactable", "Collectable", "Entity_Collision", "Object_Collision"); // Kombinierter Layer für alle Entitäten, die kollidieren oder interagiert werden können
}
