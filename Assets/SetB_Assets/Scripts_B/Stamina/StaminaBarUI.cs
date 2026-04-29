using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public PlayerStamina playerStamina;
    public Slider staminaSlider;
    public Image fillImage;

    public float lowStaminaThreshold = 20f;
    public float mediumStaminaThreshold = 50f;

    Color32 healthyColor = new Color32(111, 175, 99, 255);   // #6FAF63
    Color32 mediumColor  = new Color32(214, 193, 90, 255);   // #D6C15A
    Color32 lowColor     = new Color32(201, 106, 90, 255);   // #C96A5A

    void Update()
    {
        staminaSlider.maxValue = playerStamina.maxStamina;
        staminaSlider.value = playerStamina.currentStamina;

        UpdateBarColor();
    }

    void UpdateBarColor()
    {
        float stamina = playerStamina.currentStamina;

        if (stamina <= lowStaminaThreshold)
            fillImage.color = lowColor;
        else if (stamina <= mediumStaminaThreshold)
            fillImage.color = mediumColor;
        else
            fillImage.color = healthyColor;
    }
}