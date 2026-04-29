using UnityEngine;

/// <summary>
/// Handles player interaction input and manages the current interactable.
/// Attach this script to the Player GameObject.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private InteractionUI interactionUI;

    private Interactable currentInteractable;

    void Update()
    {
        // Check for interact input (E key)
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    /// <summary>
    /// Called by Interactable when player enters trigger zone.
    /// </summary>
    public void SetCurrentInteractable(Interactable interactable)
    {
        currentInteractable = interactable;

        // Show UI prompt
        if (interactionUI != null)
        {
            interactionUI.ShowPrompt(interactable.GetPromptText());
        }
        else
        {
            Debug.Log(interactable.GetPromptText());
        }
    }

    /// <summary>
    /// Called by Interactable when player exits trigger zone.
    /// </summary>
    public void ClearCurrentInteractable(Interactable interactable)
    {
        // Only clear if it's the same interactable (prevents issues with overlapping triggers)
        if (currentInteractable == interactable)
        {
            currentInteractable = null;

            // Hide UI prompt
            if (interactionUI != null)
            {
                interactionUI.HidePrompt();
            }
        }
    }

    /// <summary>
    /// Returns the currently focused interactable (if any).
    /// </summary>
    public Interactable GetCurrentInteractable()
    {
        return currentInteractable;
    }
}
