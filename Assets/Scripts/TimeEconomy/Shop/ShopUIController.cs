using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIController : MonoBehaviour
{
    [Header("Main References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private ShopSystem shopSystem;
    [SerializeField] private TextMeshProUGUI shopStatusText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button closeButton;

    [Header("Shop Hours")]
    [SerializeField] private int openHour = 0;
    [SerializeField] private int openMinute = 0;
    [SerializeField] private int closeHour = 23;
    [SerializeField] private int closeMinute = 59;

    [Header("Buy Tab")]
    [SerializeField] private GameObject buyTab;
    [SerializeField] private Transform buyItemContainer;

    [Header("Sell Tab")]
    [SerializeField] private GameObject sellTab;
    [SerializeField] private Transform sellItemContainer;

    [Header("Tab Buttons")]
    [SerializeField] private Button buyTabButton;
    [SerializeField] private Button sellTabButton;

    [Header("Generated UI Style")]
    [SerializeField] private float itemRowHeight = 80f;
    [SerializeField] private float itemFontSize = 28f;

    public bool IsOpenNow { get; private set; }
    public bool IsPanelOpen => shopPanel != null && shopPanel.activeSelf;

    private void Awake()
    {
        AutoFindMissingReferences();
        SetupContainers();
    }

    private void OnEnable()
    {
        GameEventSystem.timeChanged.AddListener(OnTimeChanged);
        GameEventSystem.moneyChanged.AddListener(UpdateMoneyUI);
        GameEventSystem.inventoryChanged.AddListener(RefreshCurrentTab);
        GameEventSystem.shopStockChanged.AddListener(RefreshCurrentTab);
    }

    private void OnDisable()
    {
        GameEventSystem.timeChanged.RemoveListener(OnTimeChanged);
        GameEventSystem.moneyChanged.RemoveListener(UpdateMoneyUI);
        GameEventSystem.inventoryChanged.RemoveListener(RefreshCurrentTab);
        GameEventSystem.shopStockChanged.RemoveListener(RefreshCurrentTab);
    }

    private void Start()
    {
        AutoFindMissingReferences();
        SetupContainers();
        SetupButtons();

        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (TimeSystem.Instance != null)
            OnTimeChanged(TimeSystem.Instance.Hour, TimeSystem.Instance.Minute, TimeSystem.Instance.DayNumber);
        else
            IsOpenNow = true;

        OpenBuyTab();
        UpdateMoneyUI();
        UpdateStatusText();
    }

    private void SetupButtons()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseShop);
            closeButton.onClick.AddListener(CloseShop);
        }

        if (buyTabButton != null)
        {
            buyTabButton.onClick.RemoveListener(OpenBuyTab);
            buyTabButton.onClick.AddListener(OpenBuyTab);
        }

        if (sellTabButton != null)
        {
            sellTabButton.onClick.RemoveListener(OpenSellTab);
            sellTabButton.onClick.AddListener(OpenSellTab);
        }
    }

    public void TryOpenShop()
    {
        if (!IsOpenNow)
        {
            Debug.Log("Shop is closed.");
            UpdateStatusText();
            return;
        }

        if (shopPanel != null)
            shopPanel.SetActive(true);

        OpenBuyTab();
        UpdateStatusText();
    }

    public void ToggleShop()
    {
        if (IsPanelOpen)
            CloseShop();
        else
            TryOpenShop();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);

        UpdateStatusText();
    }

    private void OnTimeChanged(int hour, int minute, int day)
    {
        IsOpenNow = IsWithinOpenHours(hour, minute);

        if (!IsOpenNow && IsPanelOpen)
            CloseShop();

        UpdateStatusText();
    }

    private bool IsWithinOpenHours(int hour, int minute)
    {
        int currentTime = hour * 60 + minute;
        int openTime = openHour * 60 + openMinute;
        int closeTime = closeHour * 60 + closeMinute;

        return currentTime >= openTime && currentTime < closeTime;
    }

    private void OpenBuyTab()
    {
        if (buyTab != null)
            buyTab.SetActive(true);

        if (sellTab != null)
            sellTab.SetActive(false);

        PopulateBuyTab();
    }

    private void OpenSellTab()
    {
        if (buyTab != null)
            buyTab.SetActive(false);

        if (sellTab != null)
            sellTab.SetActive(true);

        PopulateSellTab();
    }

    private void RefreshCurrentTab()
    {
        if (buyTab != null && buyTab.activeSelf)
        {
            PopulateBuyTab();
            return;
        }

        if (sellTab != null && sellTab.activeSelf)
        {
            PopulateSellTab();
            return;
        }

        PopulateBuyTab();
    }

    private void PopulateBuyTab()
    {
        if (shopSystem == null)
            shopSystem = ShopSystem.Instance;

        if (buyItemContainer == null)
        {
            Debug.LogWarning("Buy Item Container is not assigned.");
            return;
        }

        ClearContainer(buyItemContainer);
        SetupContainer(buyItemContainer);

        if (shopSystem == null)
        {
            CreateInfoRow(buyItemContainer, "No ShopSystem found.");
            return;
        }

        if (shopSystem.Items == null || shopSystem.Items.Count == 0)
        {
            CreateInfoRow(buyItemContainer, "No shop items found.");
            return;
        }

        for (int i = 0; i < shopSystem.Items.Count; i++)
        {
            ShopItem item = shopSystem.Items[i];
            int itemIndex = i;

            string itemName = string.IsNullOrWhiteSpace(item.itemName) ? "Unnamed Item" : item.itemName;
            string label = itemName + " | Buy: $" + item.price + " | Stock: " + item.stock;

            Button button = CreateItemButton(buyItemContainer, label);
            button.interactable = item.stock > 0;

            button.onClick.AddListener(() =>
            {
                shopSystem.BuyItem(itemIndex);
                PopulateBuyTab();
                UpdateMoneyUI();
            });
        }
    }

    private void PopulateSellTab()
    {
        if (sellItemContainer == null)
        {
            Debug.LogWarning("Sell Item Container is not assigned.");
            return;
        }

        ClearContainer(sellItemContainer);
        SetupContainer(sellItemContainer);

        if (InventorySystem.Instance == null)
        {
            CreateInfoRow(sellItemContainer, "No InventorySystem found.");
            return;
        }

        List<InventoryItemData> inventory = InventorySystem.Instance.GetInventoryData();

        if (inventory == null || inventory.Count == 0)
        {
            CreateInfoRow(sellItemContainer, "No items to sell.");
            return;
        }

        foreach (InventoryItemData inventoryItem in inventory)
        {
            if (inventoryItem == null)
                continue;

            string itemName = inventoryItem.ItemName;
            int quantity = inventoryItem.Quantity;

            if (quantity <= 0)
                continue;

            ShopItem shopItem = null;

            if (shopSystem != null && shopSystem.Items != null)
                shopItem = shopSystem.Items.Find(i => i.itemName == itemName);

            int sellPrice = shopItem != null ? shopItem.sellPrice : 0;

            string label = itemName + " x" + quantity + " | Sell: $" + sellPrice;

            Button button = CreateItemButton(sellItemContainer, label);

            button.onClick.AddListener(() =>
            {
                if (shopSystem != null)
                    shopSystem.SellItem(itemName);

                PopulateSellTab();
                UpdateMoneyUI();
            });
        }

        if (sellItemContainer.childCount == 0)
            CreateInfoRow(sellItemContainer, "No items to sell.");
    }

    private Button CreateItemButton(Transform parent, string label)
    {
        GameObject buttonObject = new GameObject("GeneratedShopItemButton");
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, itemRowHeight);

        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.minHeight = itemRowHeight;
        layoutElement.preferredHeight = itemRowHeight;
        layoutElement.flexibleWidth = 1f;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.95f, 0.78f, 0.42f, 1f);

        Button button = buttonObject.AddComponent<Button>();

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.95f, 0.78f, 0.42f, 1f);
        colors.highlightedColor = new Color(1f, 0.88f, 0.55f, 1f);
        colors.pressedColor = new Color(0.75f, 0.55f, 0.25f, 1f);
        colors.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.7f);
        button.colors = colors;

        GameObject textObject = new GameObject("Text (TMP)");
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10f, 5f);
        textRect.offsetMax = new Vector2(-10f, -5f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = itemFontSize;
        text.enableAutoSizing = true;
        text.fontSizeMin = 12f;
        text.fontSizeMax = itemFontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        text.raycastTarget = false;

        return button;
    }

    private void CreateInfoRow(Transform parent, string message)
    {
        Button button = CreateItemButton(parent, message);
        button.interactable = false;
    }

    private void ClearContainer(Transform container)
    {
        if (container == null) return;

        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }

    private void SetupContainers()
    {
        SetupContainer(buyItemContainer);
        SetupContainer(sellItemContainer);
    }

    private void SetupContainer(Transform container)
    {
        if (container == null) return;

        RectTransform rect = container.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, 0f);
        }

        VerticalLayoutGroup layout = container.GetComponent<VerticalLayoutGroup>();

        if (layout == null)
            layout = container.gameObject.AddComponent<VerticalLayoutGroup>();

        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.spacing = 10f;
        layout.padding = new RectOffset(10, 10, 10, 10);

        ContentSizeFitter fitter = container.GetComponent<ContentSizeFitter>();

        if (fitter == null)
            fitter = container.gameObject.AddComponent<ContentSizeFitter>();

        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null && MoneySystem.Instance != null)
            moneyText.text = "Money: $" + MoneySystem.Instance.CurrentMoney;
    }

    private void UpdateStatusText()
    {
        if (shopStatusText == null) return;

        if (!IsOpenNow)
            shopStatusText.text = "Shop is closed";
        else if (IsPanelOpen)
            shopStatusText.text = "Shop is open";
        else
            shopStatusText.text = "Press E to open shop";
    }

    private void AutoFindMissingReferences()
    {
        if (shopSystem == null)
            shopSystem = ShopSystem.Instance;

        if (shopSystem == null)
            shopSystem = FindObjectOfType<ShopSystem>();

        if (shopPanel == null)
        {
            GameObject found = GameObject.Find("ShopPanel");

            if (found != null)
                shopPanel = found;
        }

        if (buyTab == null)
        {
            GameObject found = GameObject.Find("BuyTab");

            if (found != null)
                buyTab = found;
        }

        if (sellTab == null)
        {
            GameObject found = GameObject.Find("SellTab");

            if (found != null)
                sellTab = found;
        }

        if (buyItemContainer == null)
        {
            GameObject found = GameObject.Find("ItemContainer");

            if (found != null)
                buyItemContainer = found.transform;
        }

        if (sellItemContainer == null)
        {
            GameObject found = GameObject.Find("SellItemContainer");

            if (found != null)
                sellItemContainer = found.transform;
        }

        if (moneyText == null)
        {
            GameObject found = GameObject.Find("MoneyText");

            if (found != null)
                moneyText = found.GetComponent<TextMeshProUGUI>();
        }

        if (shopStatusText == null)
        {
            GameObject found = GameObject.Find("ShopTitle");

            if (found != null)
                shopStatusText = found.GetComponent<TextMeshProUGUI>();
        }

        if (closeButton == null)
        {
            GameObject found = GameObject.Find("CloseButton");

            if (found != null)
                closeButton = found.GetComponent<Button>();
        }

        if (buyTabButton == null)
        {
            GameObject found = GameObject.Find("BuyTabButton");

            if (found != null)
                buyTabButton = found.GetComponent<Button>();
        }

        if (sellTabButton == null)
        {
            GameObject found = GameObject.Find("SellTabButton");

            if (found != null)
                sellTabButton = found.GetComponent<Button>();
        }
    }
}