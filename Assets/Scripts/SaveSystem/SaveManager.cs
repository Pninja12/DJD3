using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private Dictionary<string, ISaveable> saveables = new Dictionary<string, ISaveable>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        SaveablePlayer player = FindObjectOfType<SaveablePlayer>();
        if (player != null) RegisterSaveable("Player", player);

        SaveableEnemy[] enemies = FindObjectsOfType<SaveableEnemy>();
        for (int i = 0; i < enemies.Length; i++)
            RegisterSaveable("Enemy" + i, enemies[i]);

        SaveableItem[] items = FindObjectsOfType<SaveableItem>();
        for (int i = 0; i < items.Length; i++)
            RegisterSaveable("Item" + i, items[i]);
    }

    private void RegisterSaveable(string id, ISaveable saveable)
    {
        if (!saveables.ContainsKey(id))
        {
            saveables.Add(id, saveable);
        }
    }

    public void SaveGame()
    {
        Dictionary<string, object> saveData = new Dictionary<string, object>();
        foreach (var entry in saveables)
        {
            saveData.Add(entry.Key, entry.Value.GetSaveData());
        }

        string json = JsonConvert.SerializeObject(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);

        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Dictionary<string, object> saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            foreach (var entry in saveData)
            {
                if (saveables.ContainsKey(entry.Key))
                {
                    saveables[entry.Key].LoadSaveData(entry.Value);
                }
            }

            Debug.Log("Game Loaded!");
        }
        else
        {
            Debug.LogWarning("No Save Found!");
        }
    }

    public void SaveGameButton()
    {
        SaveGame();
    }

    public void LoadGameButton()
    {
        LoadGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private Dictionary<string, object> LoadFromFileOrPlayerPrefs()
    {
        return new Dictionary<string, object>(); 
    }
}