using UnityEngine;

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
        PlayerSaveData save = (PlayerSaveData)data;
        transform.position = new Vector3(save.position[0], save.position[1], save.position[2]);
    }
}
