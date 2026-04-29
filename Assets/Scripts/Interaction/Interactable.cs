using UnityEngine;

/// <summary>
/// Base class for all interactable objects in the game.
/// Attach to objects with a Collider2D set as trigger.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] protected string promptText = "[E] Interact";

    protected PlayerInteraction currentPlayer;
    protected bool isPlayerInRange = false;

    /// <summary>
    /// Called when player enters the trigger zone.
    /// </summary>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            currentPlayer = other.GetComponent<PlayerInteraction>();

            if (currentPlayer != null)
            {
                currentPlayer.SetCurrentInteractable(this);
            }
        }
    }

    /// <summary>
    /// Called when player exits the trigger zone.
    /// </summary>
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (currentPlayer != null)
            {
                currentPlayer.ClearCurrentInteractable(this);
                currentPlayer = null;
            }
        }
    }

    /// <summary>
    /// Returns the prompt text to display in the UI.
    /// </summary>
    public virtual string GetPromptText()
    {
        return promptText;
    }

    /// <summary>
    /// Called when the player presses the interact key (E).
    /// Override this in derived classes to implement specific behavior.
    /// </summary>
    public abstract void Interact();
}
