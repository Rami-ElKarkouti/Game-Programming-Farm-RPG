using UnityEngine;
using TMPro;

public class TimeDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;

    private void OnEnable()
    {
        GameEventSystem.timeChanged.AddListener(UpdateTimeDisplay);
    }

    private void OnDisable()
    {
        GameEventSystem.timeChanged.RemoveListener(UpdateTimeDisplay);
    }

    private void Start()
    {
        if (timeText == null)
            Debug.LogWarning("TimeDisplayUI: timeText is not assigned.");

        if (TimeSystem.Instance != null)
            UpdateTimeDisplay(TimeSystem.Instance.Hour, TimeSystem.Instance.Minute, TimeSystem.Instance.DayNumber);
    }

    private void UpdateTimeDisplay(int hour, int minute, int day)
    {
        if (timeText == null || TimeSystem.Instance == null) return;

        Season season = TimeSystem.Instance.CurrentSeason;
        timeText.text = $"Day {day}\n{hour:D2}:{minute:D2}\n{season}";
    }
}
