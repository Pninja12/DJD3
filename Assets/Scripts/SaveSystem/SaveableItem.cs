using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct ItemSaveData
{
    public string id;
    public bool isCollected;
}

public class SaveableItem : MonoBehaviour, ISaveable
{
    [SerializeField] private string itemId;
    [SerializeField] private bool collected = false;

    public object GetSaveData()
    {
        return new ItemSaveData { id = itemId, isCollected = collected };
    }

    public void LoadSaveData(object data)
    {
        ItemSaveData save = JsonConvert.DeserializeObject<ItemSaveData>(
            JsonConvert.SerializeObject(data)
        );

        collected = save.isCollected;
        gameObject.SetActive(!collected);
    }

    public System.Type GetSaveDataType()
    {
        return typeof(ItemSaveData);
    }

    public void MarkCollected() => collected = true;
}
