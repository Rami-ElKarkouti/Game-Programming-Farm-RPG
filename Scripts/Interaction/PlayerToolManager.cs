using UnityEngine;

public class PlayerToolManager : MonoBehaviour
{
    [Header("Current Tool")]
    [SerializeField] private ToolType equippedTool = ToolType.None;

    [Header("Tool Visuals")]
    [SerializeField] private GameObject hoeVisual;
    [SerializeField] private GameObject wateringCanVisual;
    [SerializeField] private GameObject seedsVisual;

    public ToolType EquippedTool => equippedTool;

    private void Start()
    {
        UpdateToolVisuals();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipTool(ToolType.Hoe);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipTool(ToolType.WateringCan);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipTool(ToolType.Seeds);
        }
    }

    public void EquipTool(ToolType tool)
    {
        equippedTool = tool;
        UpdateToolVisuals();
    }

    private void UpdateToolVisuals()
    {
        if (hoeVisual != null)
            hoeVisual.SetActive(equippedTool == ToolType.Hoe);

        if (wateringCanVisual != null)
            wateringCanVisual.SetActive(equippedTool == ToolType.WateringCan);

        if (seedsVisual != null)
            seedsVisual.SetActive(equippedTool == ToolType.Seeds);
    }
}