using UnityEngine;

[System.Serializable]
public class HealthManager
{
    public float maxHealth = 100f;
    public float health;

    // Constructor to initialize health-related values
    public HealthManager(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
    }

    // Take damage and reduce health
    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        Debug.Log($"Took {damage} damage! Current Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    // Heal the entity
    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        Debug.Log($"Healed {amount} health! Current Health: {health}");
    }

    // Entity dies
    private void Die()
    {
        Debug.Log("Entity has died!");
        // Handle death logic (e.g., respawn, game over, etc.)
    }
}
