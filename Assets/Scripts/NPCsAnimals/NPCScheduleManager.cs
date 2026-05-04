using UnityEngine;

public class NPCScheduleManager : MonoBehaviour
{
    [SerializeField] private MockTimeManager _mockTimeManager;
    [SerializeField] private NPCMovement[] _npcMovements;

    public void AdvanceHourAndUpdateNPCs()
    {
        if (_mockTimeManager == null)
        {
            Debug.LogWarning("MockTimeManager is not assigned.");
            return;
        }

        _mockTimeManager.AdvanceHour();

        int currentHour = _mockTimeManager.CurrentHour;

        for (int i = 0; i < _npcMovements.Length; i++)
        {
            if (_npcMovements[i] != null)
            {
                _npcMovements[i].UpdateSchedule(currentHour);
            }
        }
    }
}