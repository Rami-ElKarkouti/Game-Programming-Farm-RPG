using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interactable item that can be picked up by pressing E.
/// </summary>
public class ItemPickup : Interactable
{
    [Header("Item Settings")]
    [SerializeField] private string itemName = "Item";
    [SerializeField] private int quantity = 1;
    [SerializeField] private Sprite itemIcon;

    [Header("Pickup Behavior")]
    [SerializeField] private bool destroyOnPickup = true;

    [Header("Visual Effects")]
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobAmount = 0.1f;
    [SerializeField] private bool enableBobbing = true;

    [Header("Events")]
    [SerializeField] private UnityEvent<string, int> onPickup; // Passes item name and quantity

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Optional bobbing animation
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    public override void Interact()
    {
        PickupItem();
    }

    private void PickupItem()
    {
        Debug.Log($"Picked up: {quantity}x {itemName}");

        // Invoke pickup event (for inventory systems to listen to)
        onPickup?.Invoke(itemName, quantity);

        // TODO: Add to player inventory here
        // Example: PlayerInventory.Instance.AddItem(itemName, quantity, itemIcon);

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Get the item name.
    /// </summary>
    public string GetItemName()
    {
        return itemName;
    }

    /// <summary>
    /// Get the item quantity.
    /// </summary>
    public int GetQuantity()
    {
        return quantity;
    }

    /// <summary>
    /// Get the item icon sprite.
    /// </summary>
    public Sprite GetItemIcon()
    {
        return itemIcon;
    }
}
