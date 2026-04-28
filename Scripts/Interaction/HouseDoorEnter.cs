using UnityEngine;

public class HouseDoorEnter : MonoBehaviour
{
    [SerializeField] private Transform entryPoint;

    private bool playerInRange = false;
    private Transform playerTransform;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (playerTransform != null && entryPoint != null)
            {
                playerTransform.position = entryPoint.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
}