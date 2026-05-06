using TMPro;
using UnityEngine;

/// <summary>
/// Displays detailed information for the currently nearby animal.
/// The panel stays alive in the scene, but shows/hides itself using a CanvasGroup.
/// </summary>
public class AnimalInfoUI : MonoBehaviour
{
    // Reference to the manager that knows which animal is currently nearby.
    [SerializeField] private AnimalInteractionManager _animalInteractionManager;

    // CanvasGroup used to visually show/hide the panel without disabling the object.
    [SerializeField] private CanvasGroup _canvasGroup;

    // UI text fields for the nearby animal's details.
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _hungerText;
    [SerializeField] private TextMeshProUGUI _happinessText;
    [SerializeField] private TextMeshProUGUI _productionReadyText;

    /// <summary>
    /// Initializes the panel in a hidden state.
    /// </summary>
    private void Start()
    {
        HidePanel();
    }

    /// <summary>
    /// Updates the panel every frame based on the currently nearby animal.
    /// </summary>
    private void Update()
    {
        if (_animalInteractionManager == null)
        {
            HidePanel();
            return;
        }

        AnimalController selectedAnimal = _animalInteractionManager.GetSelectedAnimal();

        if (selectedAnimal == null)
        {
            HidePanel();
            return;
        }

        ShowPanel();

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

    /// <summary>
    /// Makes the panel visible.
    /// </summary>
    private void ShowPanel()
    {
        if (_canvasGroup == null)
        {
            return;
        }

        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hides the panel.
    /// </summary>
    private void HidePanel()
    {
        if (_canvasGroup == null)
        {
            return;
        }

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}