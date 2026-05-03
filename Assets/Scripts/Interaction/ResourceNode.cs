using System.Collections;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Save")]
    [SerializeField] private string resourceId;

    [Header("Tool Requirement")]
    [SerializeField] private string requiredToolName = "Axe";

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Visuals")]
    [SerializeField] private GameObject normalVisual;
    [SerializeField] private GameObject depletedVisual;

    [Header("Collision")]
    [SerializeField] private Collider2D[] collidersToDisableWhenDepleted;

    [Header("Reward")]
    [SerializeField] private ItemData rewardItem;
    [SerializeField] private int minRewardAmount = 2;
    [SerializeField] private int maxRewardAmount = 5;

    [Header("Regrowth")]
    [SerializeField] private float regrowSeconds = 120f;

    private int currentHealth;
    private bool depleted;

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(resourceId))
            resourceId = gameObject.scene.name + "_" + gameObject.name + "_" + transform.position.x + "_" + transform.position.y;

        currentHealth = maxHealth;

        if (normalVisual != null)
            normalVisual.SetActive(true);

        if (depletedVisual != null)
            depletedVisual.SetActive(false);
    }

    public bool TryHit(string toolName, InventoryManagern inventory)
    {
        if (depleted)
        {
            Debug.Log("This resource is depleted.");
            return false;
        }

        if (toolName != requiredToolName)
        {
            Debug.Log("Wrong tool. Need " + requiredToolName + ", but used " + toolName + ".");
            return false;
        }

        currentHealth--;

        Debug.Log(gameObject.name + " hit with " + toolName + ". HP left: " + currentHealth);

        if (currentHealth <= 0)
            Deplete(inventory);

        return true;
    }

    private void Deplete(InventoryManagern inventory)
    {
        depleted = true;
        currentHealth = 0;

        if (normalVisual != null)
            normalVisual.SetActive(false);

        if (depletedVisual != null)
            depletedVisual.SetActive(true);

        GiveReward(inventory);

        foreach (Collider2D col in collidersToDisableWhenDepleted)
        {
            if (col != null)
                col.enabled = false;
        }

        StartCoroutine(RegrowAfterDelay());
    }

    private void GiveReward(InventoryManagern inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is missing.");
            return;
        }

        if (rewardItem == null)
        {
            Debug.LogWarning(gameObject.name + " has no reward item assigned.");
            return;
        }

        int amount = Random.Range(minRewardAmount, maxRewardAmount + 1);

        for (int i = 0; i < amount; i++)
            inventory.AddItem(rewardItem);

        Debug.Log("Collected " + amount + "x " + rewardItem.itemName);
    }

    private IEnumerator RegrowAfterDelay()
    {
        yield return new WaitForSeconds(regrowSeconds);
        Regrow();
    }

    private void Regrow()
    {
        depleted = false;
        currentHealth = maxHealth;

        if (normalVisual != null)
            normalVisual.SetActive(true);

        if (depletedVisual != null)
            depletedVisual.SetActive(false);

        foreach (Collider2D col in collidersToDisableWhenDepleted)
        {
            if (col != null)
                col.enabled = true;
        }

        Debug.Log(gameObject.name + " regrew.");
    }

    public string ResourceId
    {
        get { return resourceId; }
    }

    public ResourceNodeSaveData GetSaveData()
    {
        return new ResourceNodeSaveData
        {
            resourceId = resourceId,
            currentHealth = currentHealth,
            depleted = depleted
        };
    }

    public void LoadSaveData(ResourceNodeSaveData data)
    {
        if (data == null)
            return;

        currentHealth = Mathf.Clamp(data.currentHealth, 0, maxHealth);
        depleted = data.depleted;

        if (depleted)
            currentHealth = 0;

        ApplySavedVisualState();

        Debug.Log("Resource loaded: " + resourceId);
    }

    private void ApplySavedVisualState()
    {
        if (normalVisual != null)
            normalVisual.SetActive(!depleted);

        if (depletedVisual != null)
            depletedVisual.SetActive(depleted);

        foreach (Collider2D col in collidersToDisableWhenDepleted)
        {
            if (col != null)
                col.enabled = !depleted;
        }
    }
}