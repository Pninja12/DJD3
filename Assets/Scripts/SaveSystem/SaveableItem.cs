using UnityEngine;

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
        ItemSaveData save = (ItemSaveData)data;
        collected = save.isCollected;
        gameObject.SetActive(!collected);
    }

    public void MarkCollected() => collected = true;
}

