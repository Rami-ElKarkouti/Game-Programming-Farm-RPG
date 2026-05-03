using System.Collections.Generic;
using UnityEngine;

public class HouseDoorEnter : MonoBehaviour
{
    [SerializeField] private Transform entryPoint;

    private static readonly HashSet<int> enteredHouseIds = new HashSet<int>();

    private bool playerInRange = false;
    private Transform playerTransform;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (playerTransform != null && entryPoint != null)
            {
                RegisterHouseEntry(transform);
                playerTransform.position = entryPoint.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsDoorTrigger())
        {
            playerInRange = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null;
        }
    }

    public static bool HasEnteredHouse(Transform houseMember)
    {
        Transform houseRoot = FindHouseRoot(houseMember);
        return houseRoot != null && enteredHouseIds.Contains(houseRoot.GetInstanceID());
    }

    public static void LeaveHouse(Transform houseMember)
    {
        Transform houseRoot = FindHouseRoot(houseMember);
        if (houseRoot != null)
        {
            enteredHouseIds.Remove(houseRoot.GetInstanceID());
        }
    }

    private void RegisterHouseEntry(Transform houseMember)
    {
        Transform houseRoot = FindHouseRoot(houseMember);
        if (houseRoot != null)
        {
            enteredHouseIds.Add(houseRoot.GetInstanceID());
        }
    }

    private bool IsDoorTrigger()
    {
        Collider2D doorCollider = GetComponent<Collider2D>();
        return doorCollider != null
            && doorCollider.isTrigger
            && name.Contains("DoorTrigger");
    }

    private static Transform FindHouseRoot(Transform houseMember)
    {
        Transform current = houseMember;
        while (current != null)
        {
            if (current.name.Contains("House"))
            {
                return current;
            }

            current = current.parent;
        }

        return null;
    }
}
