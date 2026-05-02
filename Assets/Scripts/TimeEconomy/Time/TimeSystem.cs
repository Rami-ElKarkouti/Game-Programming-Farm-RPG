using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField] private float realSecondsPerGameMinute = 1f;

    [Header("Current Time")]
    [SerializeField] private int hour = 6;
    [SerializeField] private int minute = 0;
    [SerializeField] private int dayNumber = 1;
    [SerializeField] private Season currentSeason = Season.Spring;

    private float timer;

    public int Hour => hour;
    public int Minute => minute;
    public int DayNumber => dayNumber;
    public Season CurrentSeason => currentSeason;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        BroadcastTimeChanged();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        while (timer >= realSecondsPerGameMinute)
        {
            timer -= realSecondsPerGameMinute;
            AdvanceMinute();
        }
    }

    private void AdvanceMinute()
    {
        minute++;

        if (minute >= 60)
        {
            minute = 0;
            hour++;
        }

        if (hour >= 24)
        {
            hour = 0;
            StartNewDay();
        }

        BroadcastTimeChanged();
    }

    private void StartNewDay()
    {
        dayNumber++;
        UpdateSeason();
        GameEventSystem.newDayStarted.Invoke();
        Debug.Log("New Day Started: Day " + dayNumber + " - " + currentSeason);
    }

    private void UpdateSeason()
    {
        int seasonIndex = ((dayNumber - 1) / 30) % 4;
        currentSeason = (Season)seasonIndex;
    }

    public void LoadTime(int loadedHour, int loadedMinute, int loadedDayNumber, Season loadedSeason)
    {
        hour = Mathf.Clamp(loadedHour, 0, 23);
        minute = Mathf.Clamp(loadedMinute, 0, 59);
        dayNumber = Mathf.Max(1, loadedDayNumber);
        currentSeason = loadedSeason;
        timer = 0f;

        BroadcastTimeChanged();
        Debug.Log("Time loaded: Day " + dayNumber + " | " + currentSeason + " | " + hour.ToString("00") + ":" + minute.ToString("00"));
    }

    private void BroadcastTimeChanged()
    {
        GameEventSystem.timeChanged.Invoke(hour, minute, dayNumber);
    }
}
