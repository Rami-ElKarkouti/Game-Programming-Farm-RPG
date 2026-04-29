using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteractable : Interactable
{
    [Header("Door Settings")]
    [SerializeField] private DoorMode doorMode = DoorMode.Teleport;

    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportDestination;

    [Header("Scene Load Settings")]
    [SerializeField] private string targetSceneName;

    public enum DoorMode
    {
        Teleport,       // Move player to another position in same scene
        LoadScene       // Load a different scene
    }

    public override void Interact()
    {
        switch (doorMode)
        {
            case DoorMode.Teleport:
                TeleportPlayer();
                break;

            case DoorMode.LoadScene:
                LoadTargetScene();
                break;
        }
    }

    private void TeleportPlayer()
    {
        if (teleportDestination == null)
        {
            Debug.LogWarning($"DoorInteractable on {gameObject.name}: No teleport destination set!");
            return;
        }

        if (currentPlayer != null)
        {
            // Teleport the player to the destination
            currentPlayer.transform.position = teleportDestination.position;
            Debug.Log($"Player teleported to {teleportDestination.name}");
        }
    }

    private void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"DoorInteractable on {gameObject.name}: No target scene name set!");
            return;
        }

        Debug.Log($"Loading scene: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }
}
