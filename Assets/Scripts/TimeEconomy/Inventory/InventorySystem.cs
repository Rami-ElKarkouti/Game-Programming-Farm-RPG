using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [SerializeField] private List<InventoryItemData> items = new List<InventoryItemData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public List<InventoryItemData> GetInventoryData()
    {
        List<InventoryItemData> copy = new List<InventoryItemData>();

        foreach (InventoryItemData item in items)
            copy.Add(new InventoryItemData(item.ItemName, item.Quantity));

        return copy;
    }

    public void LoadInventoryData(List<InventoryItemData> loadedItems)
    {
        items = new List<InventoryItemData>();

        if (loadedItems != null)
        {
            foreach (InventoryItemData item in loadedItems)
                items.Add(new InventoryItemData(item.ItemName, item.Quantity));
        }

        GameEventSystem.inventoryChanged.Invoke();
        Debug.Log("Inventory loaded. Item count: " + items.Count);
    }

    public void AddItem(string itemName, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0) return;

        InventoryItemData existingItem = items.Find(item => item.ItemName == itemName);

        if (existingItem != null)
            existingItem.Quantity += amount;
        else
            items.Add(new InventoryItemData(itemName, amount));

        GameEventSystem.inventoryChanged.Invoke();
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0) return false;

        InventoryItemData existingItem = items.Find(item => item.ItemName == itemName);

        if (existingItem == null || existingItem.Quantity < amount)
            return false;

        existingItem.Quantity -= amount;

        if (existingItem.Quantity <= 0)
            items.Remove(existingItem);

        GameEventSystem.inventoryChanged.Invoke();
        return true;
    }

    public bool HasItem(string itemName, int amount = 1)
    {
        InventoryItemData existingItem = items.Find(item => item.ItemName == itemName);
        return existingItem != null && existingItem.Quantity >= amount;
    }

    public void AddMockTestItems()
    {
        AddItem("Turnip Seeds", 5);
        AddItem("Watering Can", 1);
        AddItem("Bread", 3);
        AddItem("Apple", 4);
        Debug.Log("Mock items added.");
    }
}
