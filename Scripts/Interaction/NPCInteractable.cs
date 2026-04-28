using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interactable NPC that can trigger dialogue or events.
/// </summary>
public class NPCInteractable : Interactable
{
    [Header("NPC Settings")]
    [SerializeField] private string npcName = "NPC";

    [Header("Dialogue")]
    [SerializeField] private string[] dialogueLines;
    private int currentDialogueIndex = 0;

    [Header("Events")]
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private UnityEvent onDialogueComplete;

    [Header("Visual Feedback")]
    [SerializeField] private bool facePlayer = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        // Face the player when they enter range
        if (facePlayer && other.CompareTag("Player") && spriteRenderer != null)
        {
            bool playerOnRight = other.transform.position.x > transform.position.x;
            spriteRenderer.flipX = !playerOnRight;
        }
    }

    public override void Interact()
    {
        // Invoke custom events
        onInteract?.Invoke();

        // Show dialogue
        if (dialogueLines != null && dialogueLines.Length > 0)
        {
            ShowNextDialogue();
        }
        else
        {
            Debug.Log($"{npcName}: ...");
        }
    }

    private void ShowNextDialogue()
    {
        if (currentDialogueIndex < dialogueLines.Length)
        {
            string line = dialogueLines[currentDialogueIndex];
            Debug.Log($"{npcName}: {line}");

            currentDialogueIndex++;

            // Check if dialogue is complete
            if (currentDialogueIndex >= dialogueLines.Length)
            {
                currentDialogueIndex = 0; // Reset for next conversation
                onDialogueComplete?.Invoke();
            }
        }
    }

    /// <summary>
    /// Reset dialogue to the beginning.
    /// </summary>
    public void ResetDialogue()
    {
        currentDialogueIndex = 0;
    }

    /// <summary>
    /// Set new dialogue lines at runtime.
    /// </summary>
    public void SetDialogue(string[] newLines)
    {
        dialogueLines = newLines;
        currentDialogueIndex = 0;
    }
}
