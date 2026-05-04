using TMPro;
using UnityEngine;

public class MockTimeManager : MonoBehaviour
{
    [SerializeField] private int _currentHour = 8;
    [SerializeField] private int _currentDay = 1;

    [SerializeField] private TextMeshProUGUI _hourText;
    [SerializeField] private TextMeshProUGUI _dayText;

    public int CurrentHour => _currentHour;
    public int CurrentDay => _currentDay;

    private void Start()
    {
        UpdateUI();
    }

    public void AdvanceHour()
    {
        _currentHour++;

        if (_currentHour >= 24)
        {
            _currentHour = 0;
        }

        UpdateUI();
        Debug.Log("Hour advanced to: " + _currentHour);
    }

    public void AdvanceDay()
    {
        _currentDay++;
        UpdateUI();
        Debug.Log("Day advanced to: " + _currentDay);
    }

    private void UpdateUI()
    {
        if (_hourText != null)
        {
            _hourText.text = "Hour: " + _currentHour;
        }

        if (_dayText != null)
        {
            _dayText.text = "Day: " + _currentDay;
        }
    }
}