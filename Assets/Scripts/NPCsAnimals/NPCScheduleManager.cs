using UnityEngine;

/// <summary>
/// Listens to the shared Set C time system and updates NPC schedules when the hour changes.
/// This replaces the old mock-time button driven approach.
/// </summary>
public class NPCScheduleManager : MonoBehaviour
{
    // All NPC movement scripts that should react to time changes.
    [SerializeField] private NPCMovement[] _npcMovements;

    // Used so schedules only update once per hour.
    private int _lastProcessedHour = -1;

    /// <summary>
    /// Subscribes to Set C's timeChanged event.
    /// </summary>
    private void OnEnable()
    {
        GameEventSystem.timeChanged.AddListener(OnTimeChanged);
    }

    /// <summary>
    /// Unsubscribes from Set C's timeChanged event.
    /// </summary>
    private void OnDisable()
    {
        GameEventSystem.timeChanged.RemoveListener(OnTimeChanged);
    }

    /// <summary>
    /// Initializes NPC positions based on the current time when the scene starts.
    /// </summary>
    private void Start()
    {
        if (TimeSystem.Instance != null)
        {
            UpdateNPCSchedules(TimeSystem.Instance.Hour);
        }
    }

    /// <summary>
    /// Called whenever Set C time changes.
    /// Only updates schedules when the hour changes.
    /// </summary>
    private void OnTimeChanged(int hour, int minute, int day)
    {
        if (hour == _lastProcessedHour)
        {
            return;
        }

        UpdateNPCSchedules(hour);
    }

    /// <summary>
    /// Updates all NPCs with the new current hour.
    /// </summary>
    private void UpdateNPCSchedules(int currentHour)
    {
        _lastProcessedHour = currentHour;

        for (int i = 0; i < _npcMovements.Length; i++)
        {
            if (_npcMovements[i] != null)
            {
                _npcMovements[i].UpdateSchedule(currentHour);
            }
        }
    }


    public NPCScheduleManagerSaveData GetSaveData()
    {
        NPCScheduleManagerSaveData data = new NPCScheduleManagerSaveData();
        data.LastProcessedHour = _lastProcessedHour;
        return data;
    }

    public void LoadSaveData(NPCScheduleManagerSaveData data)
    {
        if (data == null)
        {
            return;
        }

        if (data.LastProcessedHour >= 0)
        {
            UpdateNPCSchedules(data.LastProcessedHour);
        }
        else if (TimeSystem.Instance != null)
        {
            UpdateNPCSchedules(TimeSystem.Instance.Hour);
        }
    }
}