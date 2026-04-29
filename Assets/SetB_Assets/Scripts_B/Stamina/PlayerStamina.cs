using UnityEngine;
using UnityEngine.Events;

public class PlayerStamina : MonoBehaviour
{
    public int maxStamina = 100;
    public int currentStamina;

    public UnityAction<int, int> OnStaminaChanged; // current, max

    void Awake()
    {
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    // Try to use stamina; returns true if successful
    public bool UseStamina(int amount)
    {
        if (currentStamina < amount)
        {
            Debug.Log("Not enough stamina!");
            return false;
        }

        currentStamina -= amount;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        return true;
    }

    // Restore stamina
    public void RestoreStamina(int amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina)
            currentStamina = maxStamina;

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    // Full restore (e.g., sleep or advance day)
    public void RestoreFull()
    {
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
}