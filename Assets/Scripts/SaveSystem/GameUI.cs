using UnityEngine;

public class GameUI : MonoBehaviour
{
    public void OnSaveButtonPressed()
    {
        SaveManager.Instance.SaveGame();
    }

    public void OnLoadButtonPressed()
    {
        SaveManager.Instance.LoadGame();
    }
}
