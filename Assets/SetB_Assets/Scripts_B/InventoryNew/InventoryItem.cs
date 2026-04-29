using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public Image image;
    public Text countText;

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int count = 1;
    [HideInInspector] public ItemData itemData;

    private Canvas canvas;
    
    void Awake()
    {
        image = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void InitializeItem(ItemData data)
    {
        itemData = data;

        if (image == null)
            image = GetComponent<Image>();

        if (itemData == null)
        {
            image.enabled = false;
            return;
        }

        image.enabled = true;
        image.sprite = itemData.icon;
        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = count > 1 ? count.ToString() : "";
    }

    public void ForceRefresh()
    {
        if (itemData != null)
        {
            image.enabled = true;
            image.sprite = itemData.icon;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;

        parentAfterDrag = transform.parent;

        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        if (transform.parent == canvas.transform)
        {
            transform.SetParent(parentAfterDrag);
        }
    }
}