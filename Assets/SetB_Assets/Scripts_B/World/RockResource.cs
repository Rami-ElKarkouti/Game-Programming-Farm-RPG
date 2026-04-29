using UnityEngine;

public class RockResource : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("Drop Settings")]
    public string rockItemName = "Stone";
    public int minYield = 1;
    public int maxYield = 3;

    [Header("Inventory")]
    private InventoryManagern inventory;
    private ItemData rockItem;

    private Vector3 originalScale;

    void Start()
    {
        currentHealth = maxHealth;
        originalScale = transform.localScale;

        inventory = FindFirstObjectByType<InventoryManagern>();

        if (inventory != null)
        {
            rockItem = FindItemByName(rockItemName);
        }

        if (rockItem == null)
        {
            Debug.LogError("RockResource: Missing ItemData -> " + rockItemName);
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
            BreakRock();
        }
    }

    void ResetScale()
    {
        transform.localScale = originalScale;
    }

    void BreakRock()
    {
        if (inventory == null || rockItem == null) return;

        int amount = Random.Range(minYield, maxYield + 1);

        for (int i = 0; i < amount; i++)
        {
            inventory.AddItem(rockItem);
        }

        Destroy(gameObject);
    }
}