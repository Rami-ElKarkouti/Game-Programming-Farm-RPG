using System.Collections.Generic;
using UnityEngine;

public class HouseDoorEnter : MonoBehaviour
{
    [SerializeField] private Transform entryPoint;
    [SerializeField] private float outsideDoorPadding = 0.15f;

    private static readonly HashSet<int> enteredHouseIds = new HashSet<int>();
    private static readonly Dictionary<int, HouseVisit> activeVisitsByPlayerId = new Dictionary<int, HouseVisit>();

    private bool playerInRange = false;
    private Transform playerTransform;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        if (IsDoorTrigger() && TryEjectPlayerInsideThisHouse())
        {
            return;
        }

        if (playerInRange && playerTransform != null && entryPoint != null)
        {
            RegisterHouseEntry(transform, playerTransform);
            MovePlayer(playerTransform, entryPoint.position);
            SetHouseInteriorVisible(transform, false);
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

    public static bool IsPlayerInsideHouse(Transform player, Transform houseMember)
    {
        if (player == null)
        {
            return false;
        }

        Transform houseRoot = FindHouseRoot(houseMember);
        if (houseRoot == null)
        {
            return false;
        }

        return activeVisitsByPlayerId.TryGetValue(player.GetInstanceID(), out HouseVisit visit)
            && visit.HouseRoot == houseRoot;
    }

    public static void LeaveHouse(Transform houseMember)
    {
        Transform houseRoot = FindHouseRoot(houseMember);
        if (houseRoot != null)
        {
            enteredHouseIds.Remove(houseRoot.GetInstanceID());
            RemoveVisitsForHouse(houseRoot);
            SetHouseInteriorVisible(houseRoot, true);
        }
    }

    private void RegisterHouseEntry(Transform houseMember, Transform player)
    {
        Transform houseRoot = FindHouseRoot(houseMember);
        if (houseRoot != null)
        {
            enteredHouseIds.Add(houseRoot.GetInstanceID());
            activeVisitsByPlayerId[player.GetInstanceID()] = new HouseVisit(houseRoot, transform, player);
        }
    }

    private bool TryEjectPlayerInsideThisHouse()
    {
        Transform houseRoot = FindHouseRoot(transform);
        if (houseRoot == null)
        {
            return false;
        }

        foreach (KeyValuePair<int, HouseVisit> visitEntry in activeVisitsByPlayerId)
        {
            HouseVisit visit = visitEntry.Value;
            if (visit.HouseRoot != houseRoot || visit.Player == null)
            {
                continue;
            }

            MovePlayer(visit.Player, GetOutsideDoorPosition(visit.Player));
            LeaveHouse(houseRoot);
            return true;
        }

        return false;
    }

    private Vector3 GetOutsideDoorPosition(Transform player)
    {
        Collider2D doorCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = player.GetComponent<Collider2D>();

        if (doorCollider == null)
        {
            return transform.position + Vector3.down;
        }

        float playerHalfHeight = playerCollider != null ? playerCollider.bounds.extents.y : 0f;
        return new Vector3(
            doorCollider.bounds.center.x,
            doorCollider.bounds.min.y - playerHalfHeight - outsideDoorPadding,
            player.position.z);
    }

    private static void MovePlayer(Transform player, Vector3 position)
    {
        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
        if (playerBody != null)
        {
            playerBody.linearVelocity = Vector2.zero;
            playerBody.position = position;
            return;
        }

        player.position = position;
    }

    private static void RemoveVisitsForHouse(Transform houseRoot)
    {
        List<int> visitsToRemove = new List<int>();
        foreach (KeyValuePair<int, HouseVisit> visitEntry in activeVisitsByPlayerId)
        {
            if (visitEntry.Value.HouseRoot == houseRoot)
            {
                visitsToRemove.Add(visitEntry.Key);
            }
        }

        for (int i = 0; i < visitsToRemove.Count; i++)
        {
            activeVisitsByPlayerId.Remove(visitsToRemove[i]);
        }
    }

    private static void SetHouseInteriorVisible(Transform houseMember, bool roofVisible)
    {
        Transform houseRoot = FindHouseRoot(houseMember);
        if (houseRoot == null)
        {
            return;
        }

        HouseInteriorTrigger[] interiorTriggers = houseRoot.GetComponentsInChildren<HouseInteriorTrigger>(true);
        for (int i = 0; i < interiorTriggers.Length; i++)
        {
            interiorTriggers[i].SetRoofVisible(roofVisible);
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

    private readonly struct HouseVisit
    {
        public HouseVisit(Transform houseRoot, Transform doorTrigger, Transform player)
        {
            HouseRoot = houseRoot;
            DoorTrigger = doorTrigger;
            Player = player;
        }

        public Transform HouseRoot { get; }
        public Transform DoorTrigger { get; }
        public Transform Player { get; }
    }
}
