using UnityEngine;

public class WoodResource : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Drop Settings")]
    public string woodItemName = "Wood"; 
    public int minYield = 1;
    public int maxYield = 3;

    [Header("Inventory")]
    private InventoryManagern inventory;
    private ItemData woodItem;

    private Vector3 originalScale;

    void Start()
    {
        currentHealth = maxHealth;
        originalScale = transform.localScale;

        inventory = FindFirstObjectByType<InventoryManagern>();

        if (inventory != null)
        {
            woodItem = FindItemByName(woodItemName);
        }

        if (woodItem == null)
        {
            Debug.LogError("WoodResource: Could not find ItemData named: " + woodItemName);
        }
    }

    ItemData FindItemByName(string name)
    {
        foreach (var item in inventory.allItems)
        {
            if (item.itemName == name)
                return item;
        }
        return null;
    }

    public void Hit(int damage)
    {
        currentHealth -= damage;

        transform.localScale = originalScale * 1.05f;
        Invoke(nameof(ResetScale), 0.05f);

        if (currentHealth <= 0)
        {
            ChopDown();
        }
    }

    void ResetScale()
    {
        transform.localScale = originalScale;
    }

    void ChopDown()
    {
        if (inventory == null || woodItem == null) return;

        int amount = Random.Range(minYield, maxYield + 1);

        for (int i = 0; i < amount; i++)
        {
            inventory.AddItem(woodItem);
        }

        Destroy(gameObject);
    }
}