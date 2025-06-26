using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct PlayerSaveData
{
    public float[] position;
}

public class SaveablePlayer : MonoBehaviour, ISaveable
{
    public object GetSaveData()
    {
        Vector3 pos = transform.position;
        return new PlayerSaveData { position = new float[] { pos.x, pos.y, pos.z } };
    }

    public void LoadSaveData(object data)
    {
        var jsonData = data.ToString();
        PlayerSaveData save = JsonConvert.DeserializeObject<PlayerSaveData>(jsonData);

        transform.position = new Vector3(save.position[0], save.position[1], save.position[2]);
    }

    public System.Type GetSaveDataType()
    {
        return typeof(PlayerSaveData);
    }
}
