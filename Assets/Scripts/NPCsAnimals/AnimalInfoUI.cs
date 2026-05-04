using TMPro;
using UnityEngine;

public class AnimalInfoUI : MonoBehaviour
{
    [SerializeField] private AnimalInteractionManager _animalInteractionManager;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _hungerText;
    [SerializeField] private TextMeshProUGUI _happinessText;
    [SerializeField] private TextMeshProUGUI _productionReadyText;

    private void Update()
    {
        if (_animalInteractionManager == null)
        {
            return;
        }

        AnimalController selectedAnimal = _animalInteractionManager.GetSelectedAnimal();

        if (selectedAnimal == null)
        {
            return;
        }

        if (_nameText != null)
        {
            _nameText.text = "Name: " + selectedAnimal.GetAnimalName();
        }

        if (_typeText != null)
        {
            _typeText.text = "Type: " + selectedAnimal.GetAnimalType();
        }

        if (_hungerText != null)
        {
            _hungerText.text = "Hunger: " + selectedAnimal.GetCurrentHunger();
        }

        if (_happinessText != null)
        {
            _happinessText.text = "Happiness: " + selectedAnimal.GetCurrentHappiness();
        }

        if (_productionReadyText != null)
        {
            _productionReadyText.text = "Production Ready: " + (selectedAnimal.IsProductionReady() ? "Yes" : "No");
        }
    }
}