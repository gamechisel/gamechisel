using UnityEngine;

[System.Serializable]
public class StaminaManager
{
    public float currentStamina;
    public float maxStamina;
    public float staminaRecoveryRate;

    // Constructor to initialize stamina-related values
    public StaminaManager(float maxStamina, float recoveryRate)
    {
        this.maxStamina = maxStamina;
        this.staminaRecoveryRate = recoveryRate;
        this.currentStamina = maxStamina;
    }

    // Regenerate stamina
    public void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    // Check if the entity can perform an action based on stamina cost
    public bool CanPerformAction(float staminaCost)
    {
        return currentStamina >= staminaCost;
    }

    // Perform an action, reducing stamina
    public bool PerformAction(float staminaCost)
    {
        if (CanPerformAction(staminaCost))
        {
            currentStamina -= staminaCost;
            Debug.Log($"Action performed! Remaining Stamina: {currentStamina}");
            return true;
        }
        else
        {
            Debug.Log("Not enough stamina to perform action!");
            return false;
        }
    }
}
