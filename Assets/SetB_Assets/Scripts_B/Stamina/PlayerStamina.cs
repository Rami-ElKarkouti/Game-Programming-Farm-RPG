using UnityEngine;
using UnityEngine.Events;

public class PlayerStamina : MonoBehaviour
{
    public int maxStamina = 100;
    public int currentStamina;

    public UnityAction<int, int> OnStaminaChanged;

    [SerializeField] private float regenInterval = 6f;
    [SerializeField] private int regenAmount = 3;

    private float regenTimer;

    void Awake()
    {
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    void Update()
    {
        if (currentStamina >= maxStamina)
            return;

        regenTimer += Time.deltaTime;

        if (regenTimer >= regenInterval)
        {
            regenTimer -= regenInterval;
            RestoreStamina(regenAmount);
        }
    }

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

    public void RestoreStamina(int amount)
    {
        currentStamina += amount;

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public void RestoreFull()
    {
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public int GetCurrentStamina()
    {
        return currentStamina;
    }

    public int GetMaxStamina()
    {
        return maxStamina;
    }

    public void LoadStamina(int loadedCurrentStamina, int loadedMaxStamina)
    {
        if (loadedMaxStamina > 0)
            maxStamina = loadedMaxStamina;

        currentStamina = Mathf.Clamp(loadedCurrentStamina, 0, maxStamina);
        regenTimer = 0f;

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        Debug.Log("Stamina loaded: " + currentStamina + "/" + maxStamina);
    }
}