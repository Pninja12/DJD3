using UnityEngine;

[System.Serializable]
public struct EnemySaveData
{
    public float[] position;
    public bool isDead;
}

public class SaveableEnemy : MonoBehaviour, ISaveable
{
    private PatrolAI _patrol;

    private void Awake()
    {
        _patrol = GetComponent<PatrolAI>();
    }

    public object GetSaveData()
    {
        if (this == null) return null;

        return new EnemySaveData
        {
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            isDead = _patrol.GetState() == EnemyState.Dead
        };
    }


    public void LoadSaveData(object data)
    {
        EnemySaveData save = (EnemySaveData)data;
        transform.position = new Vector3(save.position[0], save.position[1], save.position[2]);

        if (save.isDead)
        {
            _patrol.Death(); 
        }
    }
}
