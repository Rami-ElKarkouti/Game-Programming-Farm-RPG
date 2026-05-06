using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles animal interaction for Set D.
/// Instead of selecting animals with number keys,
/// the current animal is set automatically when the player enters an animal's interaction range.
/// The player can then press:
/// F = feed
/// P = pet
/// </summary>
public class AnimalInteractionManager : MonoBehaviour
{
    // Optional UI text showing which animal is currently in interaction range.
    [SerializeField] private TextMeshProUGUI _selectedAnimalText;

    // Optional UI text showing the most recent interaction result.
    [SerializeField] private TextMeshProUGUI _feedbackText;

    // The animal currently available for interaction.
    private AnimalController _currentAnimal;

    /// <summary>
    /// Clears feedback and refreshes the selected-animal UI when the scene starts.
    /// </summary>
    private void Start()
    {
        UpdateSelectedAnimalUI();
        ClearFeedback();
    }

    /// <summary>
    /// Checks for feed/pet input every frame.
    /// F and P only work when the player is currently near an animal.
    /// </summary>
    private void Update()
    {
        HandleAnimalInteraction();
    }

    /// <summary>
    /// Feeds or pets the currently nearby animal when the correct key is pressed.
    /// </summary>
    private void HandleAnimalInteraction()
    {
        if (_currentAnimal == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _currentAnimal.FeedAnimal();
            UpdateSelectedAnimalUI();
            ShowFeedback("Fed " + _currentAnimal.GetAnimalName());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _currentAnimal.PetAnimal();
            UpdateSelectedAnimalUI();
            ShowFeedback("Petted " + _currentAnimal.GetAnimalName());
        }
    }

    /// <summary>
    /// Called when the player enters an animal's trigger range.
    /// </summary>
    public void SetCurrentAnimal(AnimalController animal)
    {
        _currentAnimal = animal;
        Debug.Log("Current nearby animal set to: " + animal.GetAnimalName());
        UpdateSelectedAnimalUI();

        if (_currentAnimal != null)
        {
            ShowFeedback("Ready to interact with " + _currentAnimal.GetAnimalName());
        }
    }

    /// <summary>
    /// Called when the player leaves an animal's trigger range.
    /// Clears the current animal if it matches the one being exited.
    /// </summary>
    public void ClearCurrentAnimal(AnimalController animal)
    {
        if (_currentAnimal == animal)
        {
            _currentAnimal = null;
            UpdateSelectedAnimalUI();
            ClearFeedback();
        }
    }

    /// <summary>
    /// Returns the currently nearby animal for use by the animal info panel.
    /// </summary>
    public AnimalController GetSelectedAnimal()
    {
        return _currentAnimal;
    }

    /// <summary>
    /// Updates the UI text showing the current nearby animal.
    /// </summary>
    private void UpdateSelectedAnimalUI()
    {
        if (_selectedAnimalText == null)
        {
            return;
        }

        if (_currentAnimal == null)
        {
            _selectedAnimalText.text = "Nearby Animal: None";
            return;
        }

        _selectedAnimalText.text = "Nearby Animal: " + _currentAnimal.GetAnimalName();
    }

    /// <summary>
    /// Shows a short feedback message on screen and in the Console.
    /// </summary>
    private void ShowFeedback(string message)
    {
        if (_feedbackText != null)
        {
            _feedbackText.text = message;
        }

        Debug.Log(message);
    }

    /// <summary>
    /// Clears the feedback text.
    /// </summary>
    private void ClearFeedback()
    {
        if (_feedbackText != null)
        {
            _feedbackText.text = "";
        }
    }

    // --------------------------------------------------------------------
    // TEMPORARY SAVE COMPATIBILITY
    // --------------------------------------------------------------------
    // These methods are kept so the merged SaveSystem can still compile.
    // If your team's SaveSystem expects animal data, these can be replaced
    // later with proper animal stat save/load support.
    // --------------------------------------------------------------------

    public AnimalInteractionSaveData GetSaveData()
{
    // Set D no longer uses number-key animal selection,
    // so there is currently no important interaction state to persist here.
    // We still return an empty save object so the shared SaveSystem can compile and run.
    return new AnimalInteractionSaveData();
}

public void LoadSaveData(AnimalInteractionSaveData savedData)
{
    // Set D no longer restores a selected animal from save data.
    // Nearby-animal interaction is determined dynamically by trigger range at runtime.
    // This method remains so the shared SaveSystem can call it safely.
}
}