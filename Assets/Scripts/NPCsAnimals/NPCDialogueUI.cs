using System.Text;
using TMPro;
using UnityEngine;

public class NPCDialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    public void ShowDialogue(DialogueNode node)
    {
        if (_dialoguePanel == null)
        {
            Debug.LogError("Dialogue panel is not assigned.");
            return;
        }

        if (_dialogueText == null)
        {
            Debug.LogError("Dialogue text is not assigned.");
            return;
        }

        _dialoguePanel.SetActive(true);

        if (node == null)
        {
            _dialogueText.text = string.Empty;
            return;
        }

        StringBuilder builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(node.dialogueText))
        {
            builder.AppendLine(node.dialogueText);
        }

        bool hasChoices = node.choices != null && node.choices.Count > 0;

        if (hasChoices)
        {
            builder.AppendLine();

            for (int i = 0; i < node.choices.Count; i++)
            {
                builder.AppendLine($"{i + 1}. {node.choices[i].text}");
            }
        }
        else
        {
            builder.AppendLine();
            builder.Append("Press E to close.");
        }

        _dialogueText.text = builder.ToString();
    }

    public void HideDialogue()
    {
        if (_dialoguePanel != null)
        {
            _dialoguePanel.SetActive(false);
        }
    }
}