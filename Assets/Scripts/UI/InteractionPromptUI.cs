using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; }

    [SerializeField] private GameObject promptPanel;
    [SerializeField] private Text promptText;
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 60f, 0f);

    private Transform target;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HidePrompt();
    }

    private void LateUpdate()
    {
        if (promptPanel == null || target == null)
        {
            return;
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position);
        promptPanel.transform.position = screenPosition + screenOffset;
    }

    public void ShowPrompt(string message, Transform newTarget)
    {
        target = newTarget;

        if (promptPanel == null)
        {
            return;
        }

        if (promptText != null)
        {
            promptText.text = message;
        }

        promptPanel.SetActive(true);
    }

    public void HidePrompt()
    {
        target = null;

        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }
}