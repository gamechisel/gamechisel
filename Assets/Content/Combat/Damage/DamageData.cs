using UnityEngine;

/// <summary>
/// Datenstruktur zur Übertragung von Schadensinformationen.
/// </summary>
[System.Serializable]
public class DamageData
{
    public float damageAmount;         // Höhe des Schadens
    public Vector3 hitPosition;       // Position des Treffers
    public Vector3 hitNormal;         // Normale der getroffenen Fläche (z. B. für Effekte)
    public GameObject attacker;       // Angreifendes Objekt (z. B. Spieler, Projektil)
    public DamageType damageType;     // Typ des Schadens (z. B. Physisch, Magisch)

    /// <summary>
    /// Konstruktor für DamageData.
    /// </summary>
    /// <param name="damageAmount">Die Höhe des Schadens.</param>
    /// <param name="hitPosition">Die Position des Treffers.</param>
    /// <param name="hitNormal">Die Normale an der Trefferstelle.</param>
    /// <param name="attacker">Das angreifende Objekt.</param>
    /// <param name="damageType">Der Typ des Schadens.</param>
    public DamageData(float damageAmount, Vector3 hitPosition, Vector3 hitNormal, GameObject attacker, DamageType damageType)
    {
        this.damageAmount = damageAmount;
        this.hitPosition = hitPosition;
        this.hitNormal = hitNormal;
        this.attacker = attacker;
        this.damageType = damageType;
    }
}

/// <summary>
/// Verschiedene Typen von Schaden.
/// </summary>
public enum DamageType
{
    Physical,       // Physischer Schaden (z. B. Nahkampf, Projektil)
    Magical,        // Magischer Schaden (z. B. Zauber)
    True,           // Absoluter Schaden (keine Reduktion durch Rüstung)
    Fire,           // Feuerschaden
    Ice,            // Eis- oder Frostschaden
    Electric        // Elektrischer Schaden
}
