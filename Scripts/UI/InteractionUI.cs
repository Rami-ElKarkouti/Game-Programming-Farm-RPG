using UnityEngine;
using TMPro;

/// <summary>
/// Manages the interaction prompt UI.
/// Attach this to the InteractionPrompt panel GameObject.
/// </summary>
public class InteractionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Animation Settings (Optional)")]
    [SerializeField] private float fadeSpeed = 10f;

    private CanvasGroup canvasGroup;
    private bool isShowing = false;

    void Awake()
    {
        // Try to get CanvasGroup for smooth fading (optional)
        canvasGroup = GetComponent<CanvasGroup>();

        // Start hidden
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Optional: Smooth fade animation
        if (canvasGroup != null)
        {
            float targetAlpha = isShowing ? 1f : 0f;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Shows the interaction prompt with the given text.
    /// </summary>
    public void ShowPrompt(string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
        }

        gameObject.SetActive(true);
        isShowing = true;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f; // Start fading in
        }
    }

    /// <summary>
    /// Hides the interaction prompt.
    /// </summary>
    public void HidePrompt()
    {
        isShowing = false;

        // If no canvas group, just disable immediately
        if (canvasGroup == null)
        {
            gameObject.SetActive(false);
        }
        // Otherwise Update() will fade out, then we disable in LateUpdate
    }

    void LateUpdate()
    {
        // Disable when fully faded out
        if (canvasGroup != null && !isShowing && canvasGroup.alpha <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
