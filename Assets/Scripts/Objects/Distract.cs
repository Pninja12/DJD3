using UnityEngine;

public class Distract : MonoBehaviour
{
    [SerializeField] private float _alertRadius = 50f;

    public void DestroyObject()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _alertRadius);

        foreach(var hit in hits)
        {
            PatrolAI enemy = hit.GetComponent<PatrolAI>();
            if(enemy != null)
            {
                enemy.InvestigatePosition(transform.position);
            }
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Bullet"))
        {
            DestroyObject();
        }
    }
}
