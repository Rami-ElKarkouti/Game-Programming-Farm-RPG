using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private ShopUIController shopUIController;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Interaction Settings")]
    [SerializeField] private bool requirePlayerInRange = false;

    private bool playerInRange;

    private void Awake()
    {
        if (shopUIController == null)
            shopUIController = FindObjectOfType<ShopUIController>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(interactKey))
            return;

        if (requirePlayerInRange && !playerInRange)
        {
            Debug.Log("ShopTrigger: Player is not in range.");
            return;
        }

        if (shopUIController == null)
        {
            Debug.LogWarning("ShopTrigger: ShopUIController is not assigned.");
            return;
        }

        shopUIController.ToggleShop();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("ShopTrigger: Player entered shop range.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("ShopTrigger: Player left shop range.");
        }
    }
}