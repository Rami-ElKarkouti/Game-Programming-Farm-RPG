using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseInteriorTrigger : MonoBehaviour
{
    [SerializeField] private Tilemap roofTilemap;

    public void SetRoofVisible(bool isVisible)
    {
        if (roofTilemap != null)
        {
            roofTilemap.gameObject.SetActive(isVisible);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && HouseDoorEnter.HasEnteredHouse(transform))
        {
            SetRoofVisible(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetRoofVisible(true);
            HouseDoorEnter.LeaveHouse(transform);
        }
    }
}
