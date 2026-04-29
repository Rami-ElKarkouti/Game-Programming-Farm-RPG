using UnityEngine;
using System.Collections.Generic;

public class InventoryManagern : MonoBehaviour
{
    public InventorySlotn[] inventorySlots;
    public GameObject InventoryItemPrefab;

    [Header("Item Database (ALL items in game)")]
    public ItemData[] allItems; // drag all items to inspector

    [Header("Starter Items")]
    public ItemData[] starterItems;

    int selectedSlot = -1;

    void Start()
    {
        // PlayerPrefs.DeleteKey("Inventory"); resets inventory for testing 

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
    }

    public void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
            inventorySlots[selectedSlot].Deselect();

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(ItemData itemData)
    {
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
                return true;
            }
        }

        // empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].IsEmpty())
            {
                SpawnNewItem(itemData, inventorySlots[i]);
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
                    Destroy(item.gameObject);
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
            Destroy(item.gameObject);
        else
            item.RefreshCount();
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
            if (item.itemName == name)
                return item;
        }
        return null;
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
    }
}