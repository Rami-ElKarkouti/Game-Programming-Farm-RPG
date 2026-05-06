using UnityEngine;

public class InteractionPromptTrigger : MonoBehaviour
{
    [SerializeField] private string promptMessage = "Press E";
    [SerializeField] private Transform promptTarget;

    private void Reset()
    {
        promptTarget = transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Transform target = promptTarget != null ? promptTarget : transform;

        if (InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.ShowPrompt(promptMessage, target);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.HidePrompt();
        }
    }
}