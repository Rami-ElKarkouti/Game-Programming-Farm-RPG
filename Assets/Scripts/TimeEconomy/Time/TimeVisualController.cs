using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeVisualController : MonoBehaviour
{
    public enum TimePhase
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

    [Header("References")]
    [SerializeField] private Image backgroundImage;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 0.4f;

    [Header("Phase Colors")]
    [SerializeField] private Color morningColor = new Color(1f, 0.75f, 0.25f, 0.18f);
    [SerializeField] private Color afternoonColor = new Color(1f, 1f, 1f, 0.00f);
    [SerializeField] private Color eveningColor = new Color(1f, 0.35f, 0.05f, 0.35f);
    [SerializeField] private Color nightColor = new Color(0.0f, 0.0f, 0.08f, 0.82f);

    private TimePhase currentPhase;
    private bool hasInitialized;
    private Coroutine transitionRoutine;

    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        if (backgroundImage != null)
            backgroundImage.raycastTarget = false;
    }

    private void OnEnable()
    {
        GameEventSystem.timeChanged.AddListener(OnTimeChanged);
    }

    private void OnDisable()
    {
        GameEventSystem.timeChanged.RemoveListener(OnTimeChanged);
    }

    private void Start()
    {
        if (TimeSystem.Instance != null)
            OnTimeChanged(TimeSystem.Instance.Hour, TimeSystem.Instance.Minute, TimeSystem.Instance.DayNumber);
    }

    private void OnTimeChanged(int hour, int minute, int day)
    {
        if (backgroundImage == null)
            return;

        TimePhase newPhase = GetPhase(hour, minute);
        currentPhase = newPhase;
        hasInitialized = true;

        Color targetColor = GetSmoothColorForTime(hour, minute);

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(TransitionToColor(targetColor));
    }

    private TimePhase GetPhase(int hour, int minute)
    {
        int totalMinutes = hour * 60 + minute;

        if (totalMinutes >= 360 && totalMinutes < 720)
            return TimePhase.Morning;

        if (totalMinutes >= 720 && totalMinutes < 1020)
            return TimePhase.Afternoon;

        if (totalMinutes >= 1020 && totalMinutes < 1260)
            return TimePhase.Evening;

        return TimePhase.Night;
    }

    private Color GetSmoothColorForTime(int hour, int minute)
    {
        int totalMinutes = hour * 60 + minute;

        if (totalMinutes >= 300 && totalMinutes < 420)
        {
            float t = Mathf.InverseLerp(300, 420, totalMinutes);
            return Color.Lerp(nightColor, morningColor, t);
        }

        if (totalMinutes >= 420 && totalMinutes < 720)
        {
            float t = Mathf.InverseLerp(420, 720, totalMinutes);
            return Color.Lerp(morningColor, afternoonColor, t);
        }

        if (totalMinutes >= 720 && totalMinutes < 1020)
        {
            return afternoonColor;
        }

        if (totalMinutes >= 1020 && totalMinutes < 1260)
        {
            float t = Mathf.InverseLerp(1020, 1260, totalMinutes);
            return Color.Lerp(afternoonColor, eveningColor, t);
        }

        if (totalMinutes >= 1260 && totalMinutes < 1380)
        {
            float t = Mathf.InverseLerp(1260, 1380, totalMinutes);
            return Color.Lerp(eveningColor, nightColor, t);
        }

        return nightColor;
    }

    private Color GetColorForPhase(TimePhase phase)
    {
        switch (phase)
        {
            case TimePhase.Morning:
                return morningColor;

            case TimePhase.Afternoon:
                return afternoonColor;

            case TimePhase.Evening:
                return eveningColor;

            case TimePhase.Night:
                return nightColor;

            default:
                return morningColor;
        }
    }

    private IEnumerator TransitionToColor(Color targetColor)
    {
        Color startColor = backgroundImage.color;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            backgroundImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        backgroundImage.color = targetColor;
        transitionRoutine = null;
    }
}