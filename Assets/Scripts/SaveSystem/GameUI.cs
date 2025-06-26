using UnityEngine;

public class GameUI : MonoBehaviour
{
    private SaveManager saveManager;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        if (saveManager == null)
            Debug.LogError("SaveManager não encontrado na cena!");
    }

    public void OnSaveButtonPressed()
    {
        saveManager.SaveGame();
    }

    public void OnLoadButtonPressed()
    {
        saveManager.LoadGame();
    }
}
