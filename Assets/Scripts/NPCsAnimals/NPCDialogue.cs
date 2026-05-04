using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public int nextNodeIndex = -1;
    public int relationshipChange = 0;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea]
    public string dialogueText;

    public List<DialogueChoice> choices = new List<DialogueChoice>();
}

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private List<DialogueNode> _lowFriendshipDialogueNodes = new List<DialogueNode>();
    [SerializeField] private List<DialogueNode> _mediumFriendshipDialogueNodes = new List<DialogueNode>();
    [SerializeField] private List<DialogueNode> _highFriendshipDialogueNodes = new List<DialogueNode>();
    [SerializeField] private NPCDialogueUI _dialogueUI;

    [Header("Relationship")]
    [SerializeField] private int _maxRelationship = 10;
    [SerializeField] private int _currentRelationship = 5;

    private int _currentNodeIndex;
    private bool _isDialogueActive;

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

    private void Start()
    {

    }

    private void Update()
    {
        if (!_isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }

            return;
        }

        HandleDialogueInput();
    }

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

    private void ApplyRelationshipChange(int relationshipChange)
    {
        _currentRelationship += relationshipChange;
        _currentRelationship = Mathf.Clamp(_currentRelationship, 0, _maxRelationship);

        Debug.Log(gameObject.name + " relationship is now " + _currentRelationship + "/" + _maxRelationship);
    }

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

    private void EndDialogue()
    {
        _isDialogueActive = false;

        if (_dialogueUI != null)
        {
            _dialogueUI.HideDialogue();
        }
    }

    private bool IsValidNodeIndex(int index)
    {
        List<DialogueNode> activeDialogueNodes = GetCurrentDialogueNodes();
        return activeDialogueNodes != null && index >= 0 && index < activeDialogueNodes.Count;
    }

    public string GetRelationshipText()
    {
        return _currentRelationship + "/" + _maxRelationship;
    }

    public int GetCurrentRelationship()
    {
        return _currentRelationship;
    }

    public int GetMaxRelationship()
    {
        return _maxRelationship;
    }
}