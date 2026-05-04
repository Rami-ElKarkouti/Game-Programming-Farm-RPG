using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents one selectable dialogue response.
/// Each choice can move the conversation to another node
/// and optionally increase or decrease friendship.
/// </summary>
[System.Serializable]
public class DialogueChoice
{
    // The text shown to the player for this choice.
    public string text;

    // The index of the next dialogue node to open after this choice is selected.
    // A negative value means the dialogue should end.
    public int nextNodeIndex = -1;

    // Friendship change applied when this choice is selected.
    // Positive values increase friendship, negative values decrease it.
    public int relationshipChange = 0;
}

/// <summary>
/// Represents one dialogue node in the conversation tree.
/// A node contains one line of dialogue and an optional list of choices.
/// </summary>
[System.Serializable]
public class DialogueNode
{
    // The main dialogue text shown for this node.
    [TextArea]
    public string dialogueText;

    // The list of choices the player can select from this node.
    // If no choices exist, the player can press E to close the dialogue.
    public List<DialogueChoice> choices = new List<DialogueChoice>();
}

/// <summary>
/// Controls NPC dialogue, friendship progression, and friendship-based dialogue variation.
/// Dialogue can only start when the player is inside this NPC's interaction range.
/// </summary>
public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Sets")]
    // Dialogue used when friendship is low.
    [SerializeField] private List<DialogueNode> _lowFriendshipDialogueNodes = new List<DialogueNode>();

    // Dialogue used when friendship is medium.
    [SerializeField] private List<DialogueNode> _mediumFriendshipDialogueNodes = new List<DialogueNode>();

    // Dialogue used when friendship is high.
    [SerializeField] private List<DialogueNode> _highFriendshipDialogueNodes = new List<DialogueNode>();

    [Header("UI")]
    // Reference to the shared dialogue UI panel.
    [SerializeField] private NPCDialogueUI _dialogueUI;

    [Header("Friendship")]
    // Maximum friendship level for this NPC.
    [SerializeField] private int _maxRelationship = 10;

    // Current friendship level for this NPC.
    [SerializeField] private int _currentRelationship = 3;

    // Tracks whether the player is close enough to interact with this NPC.
    private bool _isPlayerInRange;

    // The index of the node currently being shown inside the active dialogue set.
    private int _currentNodeIndex;

    // Tracks whether this NPC is currently in an active dialogue state.
    private bool _isDialogueActive;

    private void Start()
    {
        _isDialogueActive = false;
        _isPlayerInRange = false;

        if (_dialogueUI != null)
        {
            _dialogueUI.HideDialogue();
        }
    }

    /// <summary>
    /// Handles player input every frame.
    /// Dialogue only starts when the player is inside this NPC's interaction range.
    /// </summary>
    private void Update()
    {
        if (!_isDialogueActive)
        {
            if (_isPlayerInRange && Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }

            return;
        }

        HandleDialogueInput();
    }

    /// <summary>
    /// Chooses which dialogue list should be used based on the current friendship value.
    /// Friendship ranges:
    /// 0-3  = low friendship
    /// 4-7  = medium friendship
    /// 8-10 = high friendship
    /// </summary>
    private List<DialogueNode> GetCurrentDialogueNodes()
    {
        if (_currentRelationship <= 3)
        {
            return _lowFriendshipDialogueNodes;
        }

        if (_currentRelationship <= 7)
        {
            return _mediumFriendshipDialogueNodes;
        }

        return _highFriendshipDialogueNodes;
    }

    /// <summary>
    /// Starts the dialogue from the first node of the currently selected friendship-based dialogue set.
    /// </summary>
    private void StartDialogue()
    {
        if (_dialogueUI == null)
        {
            Debug.LogError("Dialogue UI is not assigned.");
            return;
        }

        List<DialogueNode> activeDialogueNodes = GetCurrentDialogueNodes();

        if (activeDialogueNodes == null || activeDialogueNodes.Count == 0)
        {
            Debug.LogError("Dialogue nodes are empty.");
            return;
        }

        _isDialogueActive = true;
        _currentNodeIndex = 0;
        ShowCurrentNode();
    }

    /// <summary>
    /// Handles input while dialogue is active.
    /// - If the current node has choices, number keys 1..n select a choice.
    /// - If the current node has no choices, pressing E closes the dialogue.
    /// </summary>
    private void HandleDialogueInput()
    {
        if (!IsValidNodeIndex(_currentNodeIndex))
        {
            EndDialogue();
            return;
        }

        List<DialogueNode> activeDialogueNodes = GetCurrentDialogueNodes();
        DialogueNode currentNode = activeDialogueNodes[_currentNodeIndex];

        bool hasChoices = currentNode.choices != null && currentNode.choices.Count > 0;

        if (hasChoices)
        {
            for (int i = 0; i < currentNode.choices.Count; i++)
            {
                KeyCode alphaKey = (KeyCode)((int)KeyCode.Alpha1 + i);
                KeyCode keypadKey = (KeyCode)((int)KeyCode.Keypad1 + i);

                if (Input.GetKeyDown(alphaKey) || Input.GetKeyDown(keypadKey))
                {
                    SelectChoice(i);
                    return;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EndDialogue();
            }
        }
    }

    /// <summary>
    /// Processes the selected dialogue choice.
    /// Applies friendship change and moves to the next node if valid.
    /// </summary>
    private void SelectChoice(int choiceIndex)
    {
        if (!IsValidNodeIndex(_currentNodeIndex))
        {
            EndDialogue();
            return;
        }

        List<DialogueNode> activeDialogueNodes = GetCurrentDialogueNodes();
        DialogueNode currentNode = activeDialogueNodes[_currentNodeIndex];

        if (currentNode.choices == null || choiceIndex < 0 || choiceIndex >= currentNode.choices.Count)
        {
            Debug.LogWarning("Invalid dialogue choice selected.");
            return;
        }

        DialogueChoice selectedChoice = currentNode.choices[choiceIndex];
        ApplyRelationshipChange(selectedChoice.relationshipChange);

        int nextNodeIndex = selectedChoice.nextNodeIndex;

        if (nextNodeIndex < 0)
        {
            EndDialogue();
            return;
        }

        if (!IsValidNodeIndex(nextNodeIndex))
        {
            Debug.LogWarning("Next dialogue node index is invalid.");
            EndDialogue();
            return;
        }

        _currentNodeIndex = nextNodeIndex;
        ShowCurrentNode();
    }

    /// <summary>
    /// Applies friendship change and keeps the value inside the valid range.
    /// </summary>
    private void ApplyRelationshipChange(int relationshipChange)
    {
        _currentRelationship += relationshipChange;
        _currentRelationship = Mathf.Clamp(_currentRelationship, 0, _maxRelationship);

        Debug.Log(gameObject.name + " relationship is now " + _currentRelationship + "/" + _maxRelationship);
    }

    /// <summary>
    /// Displays the current node in the shared dialogue UI.
    /// </summary>
    private void ShowCurrentNode()
    {
        if (!IsValidNodeIndex(_currentNodeIndex))
        {
            EndDialogue();
            return;
        }

        List<DialogueNode> activeDialogueNodes = GetCurrentDialogueNodes();
        DialogueNode currentNode = activeDialogueNodes[_currentNodeIndex];
        _dialogueUI.ShowDialogue(currentNode);
    }

    /// <summary>
    /// Ends the current dialogue and hides the dialogue UI.
    /// </summary>
    private void EndDialogue()
    {
        _isDialogueActive = false;

        if (_dialogueUI != null)
        {
            _dialogueUI.HideDialogue();
        }
    }

    /// <summary>
    /// Checks whether a node index is valid inside the current friendship dialogue set.
    /// </summary>
    private bool IsValidNodeIndex(int index)
    {
        List<DialogueNode> activeDialogueNodes = GetCurrentDialogueNodes();
        return activeDialogueNodes != null && index >= 0 && index < activeDialogueNodes.Count;
    }

    /// <summary>
    /// Detects when the player enters this NPC's interaction trigger.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
        }
    }

    /// <summary>
    /// Detects when the player leaves this NPC's interaction trigger.
    /// If dialogue is active, it closes automatically.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;

            if (_isDialogueActive)
            {
                EndDialogue();
            }
        }
    }

    /// <summary>
    /// Returns the current friendship level as a number.
    /// Used by the friendship menu heart UI.
    /// </summary>
    public int GetCurrentRelationship()
    {
        return _currentRelationship;
    }

    /// <summary>
    /// Returns the maximum friendship level for this NPC.
    /// </summary>
    public int GetMaxRelationship()
    {
        return _maxRelationship;
    }

    /// <summary>
    /// Returns friendship as a heart string for debugging or text-based display.
    /// </summary>
    public string GetRelationshipText()
    {
        string hearts = "";

        for (int i = 0; i < _maxRelationship; i++)
        {
            if (i < _currentRelationship)
            {
                hearts += "♥";
            }
            else
            {
                hearts += "♡";
            }
        }

        return hearts;
    }
}