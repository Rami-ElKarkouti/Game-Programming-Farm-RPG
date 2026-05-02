    using UnityEngine;
using TMPro;

public class MoneyDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        GameEventSystem.moneyChanged.AddListener(UpdateMoneyDisplay);
    }

    private void OnDisable()
    {
        GameEventSystem.moneyChanged.RemoveListener(UpdateMoneyDisplay);
    }

    private void Start()
    {
        UpdateMoneyDisplay();
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyText == null || MoneySystem.Instance == null) return;

        moneyText.text = "Money: $" + MoneySystem.Instance.CurrentMoney;
    }
}
