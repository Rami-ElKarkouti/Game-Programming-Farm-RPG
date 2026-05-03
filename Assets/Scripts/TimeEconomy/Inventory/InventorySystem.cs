using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Inventory Data")]
    [SerializeField] private List<InventoryItemData> items = new List<InventoryItemData>();

    [Header("Selected Item")]
    [SerializeField] private string selectedItemName = "";

    public string SelectedItemName => selectedItemName;

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
        {
            if (item == null) continue;
            if (string.IsNullOrWhiteSpace(item.ItemName)) continue;
            if (item.Quantity <= 0) continue;

            copy.Add(new InventoryItemData(item.ItemName, item.Quantity));
        }

        return copy;
    }

    public void LoadInventoryData(List<InventoryItemData> loadedItems)
    {
        items = new List<InventoryItemData>();

        if (loadedItems != null)
        {
            foreach (InventoryItemData item in loadedItems)
            {
                if (item == null) continue;
                if (string.IsNullOrWhiteSpace(item.ItemName)) continue;
                if (item.Quantity <= 0) continue;

                items.Add(new InventoryItemData(item.ItemName, item.Quantity));
            }
        }

        if (!string.IsNullOrWhiteSpace(selectedItemName) && !HasItem(selectedItemName))
            selectedItemName = "";

        NotifyInventoryChanged();
        Debug.Log("Inventory loaded. Item count: " + items.Count);
    }

    public void LoadInventoryData(List<InventoryItemData> loadedItems, string loadedSelectedItemName)
    {
        LoadInventoryData(loadedItems);

        if (!string.IsNullOrWhiteSpace(loadedSelectedItemName) && HasItem(loadedSelectedItemName))
            selectedItemName = loadedSelectedItemName;
        else
            selectedItemName = "";

        NotifyInventoryChanged();
    }

    public void AddItem(string itemName, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0)
            return;

        InventoryItemData existingItem = items.Find(item => item.ItemName == itemName);

        if (existingItem != null)
            existingItem.Quantity += amount;
        else
            items.Add(new InventoryItemData(itemName, amount));

        NotifyInventoryChanged();
    }

    public void AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
        {
            Debug.LogWarning("InventorySystem: tried to add a null ItemData.");
            return;
        }

        AddItem(itemData.itemName, amount);
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0)
            return false;

        InventoryItemData existingItem = items.Find(item => item.ItemName == itemName);

        if (existingItem == null || existingItem.Quantity < amount)
            return false;

        existingItem.Quantity -= amount;

        if (existingItem.Quantity <= 0)
        {
            items.Remove(existingItem);

            if (selectedItemName == itemName)
                selectedItemName = "";
        }

        NotifyInventoryChanged();
        return true;
    }

    public bool RemoveItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
            return false;

        return RemoveItem(itemData.itemName, amount);
    }

    public bool HasItem(string itemName, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0)
            return false;

        InventoryItemData existingItem = items.Find(item => item.ItemName == itemName);
        return existingItem != null && existingItem.Quantity >= amount;
    }

    public bool HasItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
            return false;

        return HasItem(itemData.itemName, amount);
    }

    public int GetItemQuantity(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return 0;

        InventoryItemData existingItem = items.Find(item => item.ItemName == itemName);
        return existingItem != null ? existingItem.Quantity : 0;
    }

    public void SelectItem(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
        {
            selectedItemName = "";
            NotifyInventoryChanged();
            return;
        }

        if (!HasItem(itemName))
        {
            Debug.LogWarning("InventorySystem: cannot select item because it is not in inventory: " + itemName);
            selectedItemName = "";
            NotifyInventoryChanged();
            return;
        }

        selectedItemName = itemName;
        NotifyInventoryChanged();
        Debug.Log("Selected item: " + selectedItemName);
    }

    public void SelectItem(ItemData itemData)
    {
        if (itemData == null)
        {
            ClearSelectedItem();
            return;
        }

        SelectItem(itemData.itemName);
    }

    public void ClearSelectedItem()
    {
        selectedItemName = "";
        NotifyInventoryChanged();
    }

    public string GetSelectedItemName()
    {
        return selectedItemName;
    }

    public bool HasSelectedItem()
    {
        return !string.IsNullOrWhiteSpace(selectedItemName) && HasItem(selectedItemName);
    }

    public bool RemoveSelectedItem(int amount = 1)
    {
        if (!HasSelectedItem())
            return false;

        return RemoveItem(selectedItemName, amount);
    }

    public bool RemoveFromSelectedSlot(int amount = 1)
    {
        return RemoveSelectedItem(amount);
    }

    public void AddMockTestItems()
    {
        AddItem("Turnip Seeds", 5);
        AddItem("Watering Can", 1);
        AddItem("Bread", 3);
        AddItem("Apple", 4);
        AddItem("Hoe", 1);
        AddItem("Axe", 1);
        AddItem("Pickaxe", 1);
        AddItem("Sickle", 1);

        Debug.Log("Mock items added.");
    }

    private void NotifyInventoryChanged()
    {
        if (GameEventSystem.inventoryChanged != null)
            GameEventSystem.inventoryChanged.Invoke();
    }
}