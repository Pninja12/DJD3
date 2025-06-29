using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public static bool shouldLoadGame = false;
    private Dictionary<string, ISaveable> saveables = new Dictionary<string, ISaveable>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame()
    {
        var saveData = new Dictionary<string, object>();

        foreach (var entry in saveables)
        {
            saveData[entry.Key] = entry.Value.GetSaveData();
        }

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);
        Debug.Log("Jogo salvo!");
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.json";
        if (!File.Exists(path))
        {
            Debug.LogWarning("Nenhum arquivo de save encontrado!");
            return;
        }

        string json = File.ReadAllText(path);
        Dictionary<string, object> rawData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        foreach (var entry in rawData)
        {
            if (saveables.ContainsKey(entry.Key))
            {
                var saveable = saveables[entry.Key];
                if (saveable == null || ((MonoBehaviour)saveable) == null || ((MonoBehaviour)saveable).gameObject == null)
                {
                    Debug.LogWarning($"Objeto salvo {entry.Key} foi destruído ou não existe mais, pulando load.");
                    continue;
                }

                object typedData = JsonConvert.DeserializeObject(entry.Value.ToString(), saveable.GetSaveDataType());
                saveable.LoadSaveData(typedData);
            }
            else
            {
                Debug.LogWarning($"Objeto salvo {entry.Key} não está registrado no SaveManager.");
            }
        }

        Debug.Log("Jogo carregado!");
    }


    public void SaveGameButton()
    {
        SaveGame();
    }

    public void LoadGameButton()
    {
        shouldLoadGame = true;
        SceneManager.LoadScene("Game");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedLoad());
    }

    private IEnumerator DelayedLoad()
    {
        yield return new WaitForSeconds(0.1f); // Espera objetos instanciar

        RegisterAllSaveables();
        if(shouldLoadGame)
        {
            LoadGame();
            shouldLoadGame = false;
        }
    }

    private void RegisterAllSaveables()
    {
        saveables.Clear();

        SaveablePlayer player = FindObjectOfType<SaveablePlayer>();
        if (player != null)
            saveables.Add("Player", player);

        SaveableEnemy[] enemies = FindObjectsOfType<SaveableEnemy>();
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && enemies[i].gameObject.activeInHierarchy)
                saveables.Add("Enemy" + i, enemies[i]);
        }

        SaveableItem[] items = FindObjectsOfType<SaveableItem>();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].gameObject.activeInHierarchy)
                saveables.Add("Item" + i, items[i]);
        }
    }

}
