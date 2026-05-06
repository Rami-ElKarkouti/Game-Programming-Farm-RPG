using UnityEngine;

public class FriendshipHouseTrigger : MonoBehaviour
{
    [SerializeField] private FriendshipMenuUI friendshipMenu;
    [SerializeField] private bool closeWhenLeavingHouse = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (friendshipMenu != null)
        {
            friendshipMenu.OpenMenu();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (closeWhenLeavingHouse && friendshipMenu != null)
        {
            friendshipMenu.CloseMenu();
        }
    }
}