using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NPCScheduleEntry
{
    public int hour;
    public Transform targetLocation;
}

public class NPCMovement : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void SetWalkingAnimation(bool isWalking)
    {
        if (_animator != null)
        {
            _animator.SetBool("IsWalking", isWalking);
        }
    }
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private List<NPCScheduleEntry> _scheduleEntries = new List<NPCScheduleEntry>();

    private Transform _currentTarget;

    public void UpdateSchedule(int currentHour)
    {
        for (int i = 0; i < _scheduleEntries.Count; i++)
        {
            if (_scheduleEntries[i].hour == currentHour)
            {
                _currentTarget = _scheduleEntries[i].targetLocation;
                Debug.Log(gameObject.name + " target changed to " + _currentTarget.name);
                return;
            }
        }
    }

    private void Update()
    {
        if (_currentTarget == null)
        {
            SetWalkingAnimation(false);
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, _currentTarget.position);

        if (distanceToTarget > 0.05f)
        {
            SetWalkingAnimation(true);

            transform.position = Vector2.MoveTowards(
                transform.position,
                _currentTarget.position,
                _moveSpeed * Time.deltaTime
            );
        }
        else
        {
            SetWalkingAnimation(false);
        }
    }
}