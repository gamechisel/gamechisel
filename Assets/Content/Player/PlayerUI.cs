using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    // References to the UI elements
    [Header("Health and Stamina")]
    public Slider healthBar;
    public Slider staminaBar;
    public TMP_Text healthBottlesAmountText;
    public TMP_Text throwableAmountText;

    // [Header("Special Orbs")]
    // public TMP_Text orbsText;

    // [Header("Weapons and Effects")]
    // public TMP_Text weaponsText;
    // public TMP_Text effectsText;

    // [Header("Quest and In-Game Time")]
    // public TMP_Text questText;
    // public TMP_Text timeText;


    // Methods to update the UI

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth / maxHealth;
    }

    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        staminaBar.value = currentStamina / maxStamina;
    }

    public void UpdateHealthBottles(int healthBottles)
    {
        healthBottlesAmountText.text = healthBottles.ToString();
    }

    public void UpdateThrowableAmount(int throwables)
    {
        throwableAmountText.text = throwables.ToString();
    }

    // public void UpdateOrbs(int currentOrbs)
    // {
    //     orbsText.text = $"Orbs: {currentOrbs}";
    // }

    // public void UpdateWeapons(string weaponName)
    // {
    //     weaponsText.text = $"Weapon: {weaponName}";
    // }

    // public void UpdateEffects(string[] effects)
    // {
    //     effectsText.text = "Effects: " + string.Join(", ", effects);
    // }

    // public void UpdateQuest(string questDescription)
    // {
    //     questText.text = $"Quest: {questDescription}";
    // }

    // public void UpdateTime(string inGameTime)
    // {
    //     timeText.text = $"Time: {inGameTime}";
    // }

    // Optional: Method for general updates when multiple things are changed at once
    public void UpdateAllUI(float currentHealth, float maxHealth, float currentStamina, float maxStamina, int healthBottles, int throwableAmount)
    {
        UpdateHealth(currentHealth, maxHealth);
        UpdateStamina(currentStamina, maxStamina);
        UpdateHealthBottles(healthBottles);
        UpdateThrowableAmount(throwableAmount);
        // UpdateOrbs(currentOrbs);
        // UpdateWeapons(weaponName);
        // UpdateEffects(effects);
        // UpdateQuest(questDescription);
        // UpdateTime(inGameTime);
    }
}
