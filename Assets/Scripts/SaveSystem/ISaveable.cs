public interface ISaveable
{
    object GetSaveData();
    void LoadSaveData(object data);
    System.Type GetSaveDataType(); 
}

