using UnityEngine;
using UnityEngine.UI;

public class FriendshipMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;

    [Header("NPC References")]
    [SerializeField] private NPCDialogue _shopkeeperNPC;
    [SerializeField] private NPCDialogue _farmerNPC;
    [SerializeField] private NPCDialogue _villagerNPC;

    [Header("Shopkeeper Hearts")]
    [SerializeField] private Image[] _shopkeeperHearts;

    [Header("Farmer Hearts")]
    [SerializeField] private Image[] _farmerHearts;

    [Header("Villager Hearts")]
    [SerializeField] private Image[] _villagerHearts;

    private void Start()
    {
        if (_menuPanel != null)
        {
            _menuPanel.SetActive(false);
        }
    }

   private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _menuPanel.SetActive(false);
            Debug.Log("FORCED OFF");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            _menuPanel.SetActive(true);
            Debug.Log("FORCED ON");

            RefreshMenu(); 
        }
    }

    private void ToggleMenu()
    {
        if (_menuPanel == null)
        {
            return;
        }

        bool isActive = !_menuPanel.activeSelf;
        _menuPanel.SetActive(isActive);

        if (isActive)
        {
            RefreshMenu();
        }
    }

    private void RefreshMenu()
    {
        if (_shopkeeperNPC != null)
        {
            UpdateHeartRow(_shopkeeperHearts, _shopkeeperNPC.GetCurrentRelationship());
        }

        if (_farmerNPC != null)
        {
            UpdateHeartRow(_farmerHearts, _farmerNPC.GetCurrentRelationship());
        }

        if (_villagerNPC != null)
        {
            UpdateHeartRow(_villagerHearts, _villagerNPC.GetCurrentRelationship());
        }
    }

    private void UpdateHeartRow(Image[] hearts, int currentRelationship)
    {
        if (hearts == null)
        {
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
            {
                continue;
            }

            Color heartColor = hearts[i].color;

            if (i < currentRelationship)
            {
                heartColor.a = 1f;
            }
            else
            {
                heartColor.a = 0.2f;
            }

            hearts[i].color = heartColor;
        }
    }
}