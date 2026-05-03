using UnityEngine;
using System.Collections.Generic;

public class InventoryManagern : MonoBehaviour
{
    public static InventoryManagern Instance { get; private set; }

    public InventorySlotn[] inventorySlots;
    public GameObject InventoryItemPrefab;

    [Header("Item Database (ALL items in game)")]
    public ItemData[] allItems; // drag all items to inspector

    [Header("Starter Items")]
    public ItemData[] starterItems;

    int selectedSlot = -1;

    public ItemData CurrentSelectedItem { get; private set; }

    [SerializeField] private bool resetInventoryOnStart = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            Debug.LogError("❌ No inventory slots assigned!");
            return;
        }

        ClearAllSlots();

        if (PlayerPrefs.HasKey("Inventory"))
        {
            LoadInventory();
        }
        else
        {
            GiveStarterItems();
            SaveInventory();
        }

        ChangeSelectedSlot(0);
    }

    void ClearAllSlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            foreach (Transform child in inventorySlots[i].transform)
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log("🧹 All inventory slots cleared");
    }

    void OnApplicationQuit()
    {
        SaveInventory();
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 8)
            {
                ChangeSelectedSlot(number - 1);
            }
        }

        // Reset Inventory with O key (for testing)
        if (Input.GetKeyDown(KeyCode.O))
        {
            ResetInventory();
        }
    }

    public void ChangeSelectedSlot(int newValue)
    {
        if (newValue < 0 || newValue >= inventorySlots.Length)
            return;

        if (selectedSlot >= 0)
            inventorySlots[selectedSlot].Deselect();

        inventorySlots[newValue].Select();
        selectedSlot = newValue;

        UpdateSelectedItem();
    }

    private void UpdateSelectedItem()
    {
        if (selectedSlot < 0 || selectedSlot >= inventorySlots.Length)
        {
            CurrentSelectedItem = null;
            return;
        }

        InventoryItem item = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();

        CurrentSelectedItem = (item != null && item.count > 0) ? item.itemData : null;
    }

    public bool AddItem(ItemData itemData)
    {
        if (itemData == null)
            return false;

        // stack
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null &&
                itemData.stackable &&
                itemInSlot.itemData == itemData &&
                itemInSlot.count < itemData.maxStack)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();

                UpdateSelectedItem();
                return true;
            }
        }

        // empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].IsEmpty())
            {
                SpawnNewItem(itemData, inventorySlots[i]);

                UpdateSelectedItem();
                return true;
            }
        }

        return false;
    }

    void SpawnNewItem(ItemData itemData, InventorySlotn slot)
    {
        GameObject newItem = Instantiate(InventoryItemPrefab, slot.transform);
        InventoryItem item = newItem.GetComponent<InventoryItem>();
        item.InitializeItem(itemData);
    }

    public ItemData GetSelectedItem(bool use)
    {
        if (selectedSlot < 0) return null;

        InventoryItem item = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();

        if (item != null)
        {
            ItemData data = item.itemData;

            if (use)
            {
                item.count--;

                if (item.count <= 0)
                {
                    Destroy(item.gameObject);
                    UpdateSelectedItem();
                }
                else
                    item.RefreshCount();
            }

            return data;
        }

        return null;
    }

    public void RemoveFromSelectedSlot(int amount)
    {
        if (selectedSlot < 0) return;

        InventoryItem item = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();

        if (item == null) return;

        item.count -= amount;

        if (item.count <= 0)
        {
            Destroy(item.gameObject);
            UpdateSelectedItem();
        }
        else
        {
            item.RefreshCount();
        }

        UpdateSelectedItem();
    }

    public bool HasItem(ItemData itemData)
    {
        foreach (var slot in inventorySlots)
        {
            InventoryItem item = slot.GetComponentInChildren<InventoryItem>();

            if (item != null && item.itemData == itemData)
                return true;
        }
        return false;
    }

    // SAVE SYSTEM

    void SaveInventory()
    {
        List<InventorySaveData> saveData = new List<InventorySaveData>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem item = inventorySlots[i].GetComponentInChildren<InventoryItem>();

            if (item != null)
            {
                InventorySaveData data = new InventorySaveData
                {
                    itemName = item.itemData.itemName,
                    count = item.count,
                    slotIndex = i
                };

                saveData.Add(data);
            }
        }

        string json = JsonUtility.ToJson(new Wrapper { items = saveData });
        PlayerPrefs.SetString("Inventory", json);
        PlayerPrefs.Save();
    }

    void LoadInventory()
    {
        ClearAllSlots(); // 🔥 ADD THIS FIRST

        if (!PlayerPrefs.HasKey("Inventory")) return;

        string json = PlayerPrefs.GetString("Inventory");
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);

        foreach (var data in wrapper.items)
        {
            ItemData itemData = FindItemByName(data.itemName);

            if (itemData != null)
            {
                SpawnNewItem(itemData, inventorySlots[data.slotIndex]);

                InventoryItem item = inventorySlots[data.slotIndex].GetComponentInChildren<InventoryItem>();
                item.count = data.count;
                item.RefreshCount();
            }
        }
    }

    ItemData FindItemByName(string name)
    {
        foreach (var item in allItems)
        {
            if (item != null && item.itemName == name)
                return item;
        }
        return null;
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }

    public List<InventorySaveData> GetInventoryData()
    {
        List<InventorySaveData> saveData = new List<InventorySaveData>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem item = inventorySlots[i].GetComponentInChildren<InventoryItem>();

            if (item != null && item.itemData != null && item.count > 0)
            {
                InventorySaveData data = new InventorySaveData
                {
                    itemName = item.itemData.itemName,
                    count = item.count,
                    slotIndex = i
                };

                saveData.Add(data);
            }
        }

        return saveData;
    }

    public void LoadInventoryData(List<InventorySaveData> loadedItems, int loadedSelectedSlot)
    {
        ClearAllSlots();

        if (loadedItems != null)
        {
            foreach (InventorySaveData data in loadedItems)
            {
                if (data == null) continue;
                if (string.IsNullOrWhiteSpace(data.itemName)) continue;
                if (data.count <= 0) continue;
                if (data.slotIndex < 0 || data.slotIndex >= inventorySlots.Length) continue;

                ItemData itemData = FindItemByName(data.itemName);

                if (itemData == null)
                {
                    Debug.LogWarning("InventoryManagern: could not find item while loading JSON: " + data.itemName);
                    continue;
                }

                SpawnNewItem(itemData, inventorySlots[data.slotIndex]);

                InventoryItem item = inventorySlots[data.slotIndex].GetComponentInChildren<InventoryItem>();

                if (item != null)
                {
                    item.count = data.count;
                    item.RefreshCount();
                }
            }
        }

        if (loadedSelectedSlot >= 0 && loadedSelectedSlot < inventorySlots.Length)
            ChangeSelectedSlot(loadedSelectedSlot);
        else
            ChangeSelectedSlot(0);

        UpdateSelectedItem();
        Debug.Log("Inventory loaded from JSON SaveSystem.");
    }

    public bool AddItemByName(string itemName, int amount = 1)
    {
        ItemData itemData = FindItemByName(itemName);

        if (itemData == null)
        {
            Debug.LogWarning("InventoryManagern: item not found: " + itemName);
            return false;
        }

        bool success = true;

        for (int i = 0; i < amount; i++)
        {
            if (!AddItem(itemData))
                success = false;
        }

        return success;
    }

    public bool RemoveItemByName(string itemName, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0)
            return false;

        if (GetItemQuantity(itemName) < amount)
            return false;

        int remaining = amount;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem item = inventorySlots[i].GetComponentInChildren<InventoryItem>();

            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.itemName != itemName)
                continue;

            int removed = Mathf.Min(item.count, remaining);
            item.count -= removed;
            remaining -= removed;

            if (item.count <= 0)
                Destroy(item.gameObject);
            else
                item.RefreshCount();

            if (remaining <= 0)
                break;
        }

        UpdateSelectedItem();
        return true;
    }

    public int GetItemQuantity(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return 0;

        int total = 0;

        foreach (InventorySlotn slot in inventorySlots)
        {
            InventoryItem item = slot.GetComponentInChildren<InventoryItem>();

            if (item != null && item.itemData != null && item.itemData.itemName == itemName)
                total += item.count;
        }

        return total;
    }

    public ItemData GetItemDataByName(string itemName)
    {
        return FindItemByName(itemName);
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<InventorySaveData> items;
    }

    void GiveStarterItems()
    {
        foreach (var item in starterItems)
        {
            AddItem(item);
        }

        Debug.Log("Starter items added!");
    }

    public void DeleteSelectedItem()
    {
        if (selectedSlot < 0) return;

        InventoryItem item = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();

        if (item == null) return;

        item.count--;

        if (item.count <= 0)
        {
            Destroy(item.gameObject);
        }
        else
        {
            item.RefreshCount();
        }

        UpdateSelectedItem();
    }

    public void ResetInventory()
    {
        PlayerPrefs.DeleteKey("Inventory");
        PlayerPrefs.Save();

        Debug.Log("🧹 Manual Inventory Reset");

        ClearAllSlots();
        GiveStarterItems();
        SaveInventory();

        ChangeSelectedSlot(0);
    }
}