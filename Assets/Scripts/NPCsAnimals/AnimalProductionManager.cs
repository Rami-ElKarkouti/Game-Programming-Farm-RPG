using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles daily animal production using the shared Set C day system.
/// When Set C starts a new day, animals advance one day, may produce items,
/// and produced items are added to the shared inventory.
/// </summary>
public class AnimalProductionManager : MonoBehaviour
{
    // All animals that should be processed at the start of each new day.
    [SerializeField] private AnimalController[] _animals;

    // UI text used to display today's production and cumulative totals.
    [SerializeField] private TextMeshProUGUI _productionLogText;

    [SerializeField] private TextMeshProUGUI _eggCountText;
[SerializeField] private TextMeshProUGUI _milkCountText;
[SerializeField] private TextMeshProUGUI _woolCountText;

private void UpdateProductionTotalsUI()
{
    if (_eggCountText != null)
    {
        int eggCount = _productionTotals.ContainsKey("Egg") ? _productionTotals["Egg"] : 0;
        _eggCountText.text = "Egg: " + eggCount;
    }

    if (_milkCountText != null)
    {
        int milkCount = _productionTotals.ContainsKey("Milk") ? _productionTotals["Milk"] : 0;
        _milkCountText.text = "Milk: " + milkCount;
    }

    if (_woolCountText != null)
    {
        int woolCount = _productionTotals.ContainsKey("Wool") ? _productionTotals["Wool"] : 0;
        _woolCountText.text = "Wool: " + woolCount;
    }
}

    // Running total of produced items across all days.
    private Dictionary<string, int> _productionTotals = new Dictionary<string, int>();

    /// <summary>
    /// Subscribes to Set C's new-day event.
    /// </summary>
    private void OnEnable()
{
    Debug.Log("AnimalProductionManager OnEnable ran");
    GameEventSystem.newDayStarted.AddListener(HandleNewDay);
}

    /// <summary>
    /// Unsubscribes from Set C's new-day event.
    /// </summary>
private void OnDisable()
{
    Debug.Log("AnimalProductionManager OnDisable ran");
    GameEventSystem.newDayStarted.RemoveListener(HandleNewDay);
}

    /// <summary>
    /// Called automatically whenever Set C starts a new day.
    /// This advances all animals by one day, processes production,
    /// updates totals, and adds produced items to inventory.
    /// </summary>
    private void HandleNewDay()
    {
        Debug.Log("AnimalProductionManager: Set C new day event received");

        string dailyLog = "Today:\n";

        for (int i = 0; i < _animals.Length; i++)
        {
            if (_animals[i] == null)
            {
                continue;
            }

            _animals[i].AdvanceDay();

            if (_animals[i].CanProduce())
{
    string item = _animals[i].GetProductionItem();

    if (!_productionTotals.ContainsKey(item))
    {
        _productionTotals[item] = 0;
    }

    _productionTotals[item]++;

    if (InventorySystem.Instance != null)
    {
        InventorySystem.Instance.AddItem(item, 1);

        int newQuantity = InventorySystem.Instance.GetItemQuantity(item);
        Debug.Log("Added to inventory: " + item + " | New quantity = " + newQuantity);
    }
    else
    {
        Debug.LogWarning("InventorySystem instance not found. Produced item was not added to inventory.");
    }

    dailyLog += _animals[i].GetAnimalName() + " produced " + item + "\n";
}
            else
            {
                dailyLog += _animals[i].GetAnimalName() + " was too hungry to produce\n";
            }
        }

        UpdateProductionLogUI(dailyLog);
        UpdateProductionTotalsUI();
        Debug.Log(dailyLog);
    }

    /// <summary>
    /// Updates the production panel with today's results and cumulative totals.
    /// </summary>
    private void UpdateProductionLogUI(string dailyLog)
    {
        if (_productionLogText == null)
        {
            return;
        }

        string totalsText = "\nTotals:\n";

        foreach (KeyValuePair<string, int> entry in _productionTotals)
        {
            totalsText += entry.Key + ": " + entry.Value + "\n";
        }

        _productionLogText.text = dailyLog + totalsText;
    }

    public AnimalProductionSaveData GetSaveData()
    {
        AnimalProductionSaveData data = new AnimalProductionSaveData();

        foreach (KeyValuePair<string, int> entry in _productionTotals)
        {
            ProductionTotalSaveData totalData = new ProductionTotalSaveData();
            totalData.ItemName = entry.Key;
            totalData.Quantity = entry.Value;
            data.ProductionTotals.Add(totalData);
        }

        return data;
    }

    public void LoadSaveData(AnimalProductionSaveData data)
    {
        _productionTotals.Clear();

        if (data == null || data.ProductionTotals == null)
        {
            UpdateProductionLogUI("Today:\n");
            return;
        }

        for (int i = 0; i < data.ProductionTotals.Count; i++)
        {
            ProductionTotalSaveData totalData = data.ProductionTotals[i];

            if (totalData == null || string.IsNullOrEmpty(totalData.ItemName))
            {
                continue;
            }

            _productionTotals[totalData.ItemName] = totalData.Quantity;
        }

        UpdateProductionLogUI("Today:\nLoaded saved animal production totals.\n");
        UpdateProductionTotalsUI();
    }
}