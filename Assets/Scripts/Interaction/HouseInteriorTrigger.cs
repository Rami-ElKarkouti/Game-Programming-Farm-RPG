using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseInteriorTrigger : MonoBehaviour
{
    [SerializeField] private Tilemap roofTilemap;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && roofTilemap != null)
        {
            roofTilemap.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && roofTilemap != null)
        {
            roofTilemap.gameObject.SetActive(true);
        }
    }
}