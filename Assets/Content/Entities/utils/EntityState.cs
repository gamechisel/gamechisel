using UnityEngine;

[System.Serializable]
public class EntityState
{
    // Entity-specific values
    public HealthManager healthManager;
    public StaminaManager staminaManager;

    void Start()
    {
        // Initialize HealthManager
        healthManager = new HealthManager(maxHealth: 100f);
        staminaManager = new StaminaManager(maxStamina: 100f, recoveryRate: 5f);
    }

    void Update()
    {
        // Update stamina regeneration every frame
        staminaManager.RegenerateStamina();
    }

    // Take damage through the HealthManager
    public void TakeDamage(float damage)
    {
        healthManager.TakeDamage(damage);
    }

    // Heal the entity using HealthManager
    public void Heal(float amount)
    {
        healthManager.Heal(amount);
    }

    // Perform an action using the StaminaManager
    public bool PerformAction(float staminaCost)
    {
        return staminaManager.PerformAction(staminaCost);
    }
}
