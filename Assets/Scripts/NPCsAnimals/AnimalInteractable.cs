using UnityEngine;

/// <summary>
/// Connects an animal to the shared player interaction/trigger system.
/// When the player enters the trigger, this animal becomes the current animal
/// in the AnimalInteractionManager.
/// It also uses the existing Interactable prompt system from the shared project.
/// </summary>
public class AnimalInteractable : Interactable
{
    // Reference to the actual animal logic on this object.
    [SerializeField] private AnimalController _animalController;

    // Reference to the Set D animal interaction manager.
    [SerializeField] private AnimalInteractionManager _animalInteractionManager;

    /// <summary>
    /// Finds missing references automatically when possible.
    /// </summary>
    private void Awake()
    {
        if (_animalController == null)
        {
            _animalController = GetComponent<AnimalController>();
        }

        if (_animalInteractionManager == null)
        {
            _animalInteractionManager = FindFirstObjectByType<AnimalInteractionManager>();
        }
    }

    /// <summary>
    /// Called when the player enters this animal's trigger.
    /// Uses the base interactable logic for prompt display,
    /// then tells Set D which animal is currently nearby.
    /// </summary>
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Player") && _animalInteractionManager != null && _animalController != null)
        {
            _animalInteractionManager.SetCurrentAnimal(_animalController);
        }
    }

    /// <summary>
    /// Called when the player leaves this animal's trigger.
    /// Uses the base interactable logic to clear the prompt,
    /// then clears the current Set D animal if needed.
    /// </summary>
    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        if (other.CompareTag("Player") && _animalInteractionManager != null && _animalController != null)
        {
            _animalInteractionManager.ClearCurrentAnimal(_animalController);
        }
    }

    /// <summary>
    /// Returns the prompt shown when the player is near the animal.
    /// This uses the shared InteractionUI system.
    /// </summary>
    public override string GetPromptText()
    {
        if (_animalController != null)
        {
            return "[F] Feed   [P] Pet   " + _animalController.GetAnimalName();
        }

        return "[F] Feed   [P] Pet";
    }

    /// <summary>
    /// Required by the shared Interactable base class.
    /// We do not use E for animals right now, so this stays empty.
    /// </summary>
    public override void Interact()
    {
        // Intentionally left empty.
        // Animal interaction uses F and P while the player is in range.
    }
}