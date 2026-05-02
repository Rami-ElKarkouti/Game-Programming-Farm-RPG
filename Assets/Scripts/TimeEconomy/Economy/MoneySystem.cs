using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem Instance { get; private set; }

    [SerializeField] private int startingMoney = 500;
    [SerializeField] private int currentMoney;

    public int CurrentMoney => currentMoney;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentMoney = startingMoney;
    }

    private void Start()
    {
        GameEventSystem.moneyChanged.Invoke();
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        currentMoney += amount;
        GameEventSystem.moneyChanged.Invoke();
        Debug.Log("Money added: " + amount + " | Total: " + currentMoney);
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0) return true;

        if (currentMoney < amount)
            return false;

        currentMoney -= amount;
        GameEventSystem.moneyChanged.Invoke();
        Debug.Log("Money spent: " + amount + " | Total: " + currentMoney);
        return true;
    }

    public void LoadMoney(int loadedMoney)
    {
        currentMoney = Mathf.Max(0, loadedMoney);
        GameEventSystem.moneyChanged.Invoke();
        Debug.Log("Money loaded: " + currentMoney);
    }
}
