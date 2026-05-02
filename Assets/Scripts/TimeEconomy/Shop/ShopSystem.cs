using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }

    [SerializeField] private List<ShopItem> items = new List<ShopItem>();

    private readonly List<int> originalStock = new List<int>();

    public List<ShopItem> Items => items;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameEventSystem.newDayStarted.AddListener(RestockShop);
    }

    private void OnDisable()
    {
        GameEventSystem.newDayStarted.RemoveListener(RestockShop);
    }

    private void Start()
    {
        CacheOriginalStock();
    }

    private void CacheOriginalStock()
    {
        originalStock.Clear();

        foreach (ShopItem item in items)
            originalStock.Add(item.stock);
    }

    private void RestockShop()
    {
        for (int i = 0; i < items.Count && i < originalStock.Count; i++)
            items[i].stock = originalStock[i];

        GameEventSystem.shopStockChanged.Invoke();
        Debug.Log("Shop restocked for new day.");
    }

    public bool BuyItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning("Invalid shop item index: " + index);
            return false;
        }

        ShopItem item = items[index];

        if (item.stock <= 0)
        {
            Debug.Log("Item out of stock: " + item.itemName);
            return false;
        }

        if (MoneySystem.Instance == null || !MoneySystem.Instance.SpendMoney(item.price))
        {
            Debug.Log("Not enough money.");
            return false;
        }

        if (InventorySystem.Instance != null)
            InventorySystem.Instance.AddItem(item.itemName, 1);

        item.stock--;
        GameEventSystem.shopStockChanged.Invoke();
        Debug.Log("Bought: " + item.itemName + " | Stock left: " + item.stock);
        return true;
    }

    public bool SellItem(string itemName)
    {
        if (InventorySystem.Instance == null || MoneySystem.Instance == null)
            return false;

        ShopItem item = items.Find(i => i.itemName == itemName);

        if (!InventorySystem.Instance.RemoveItem(itemName, 1))
        {
            Debug.Log("Item not in inventory: " + itemName);
            return false;
        }

        int sellPrice = item != null ? item.sellPrice : 0;
        MoneySystem.Instance.AddMoney(sellPrice);
        Debug.Log("Sold: " + itemName + " for $" + sellPrice);
        return true;
    }

    public List<int> GetStockData()
    {
        List<int> stock = new List<int>();

        foreach (ShopItem item in items)
            stock.Add(item.stock);

        return stock;
    }

    public void LoadStockData(List<int> stockData)
    {
        if (stockData == null) return;

        for (int i = 0; i < items.Count && i < stockData.Count; i++)
            items[i].stock = stockData[i];

        GameEventSystem.shopStockChanged.Invoke();
        Debug.Log("Shop stock loaded.");
    }
}
