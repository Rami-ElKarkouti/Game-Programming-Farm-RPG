using UnityEngine;

public class SaveLoadUI : MonoBehaviour
{
    public void SaveGame()
    {
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.SaveGame();
    }

    public void LoadGame()
    {
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.LoadGame();
    }

    public void DeleteSave()
    {
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.DeleteSave();
    }
}
