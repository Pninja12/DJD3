using UnityEngine;

public class Bullet : MonoBehaviour
{
   private float life = 3;

    void Awake()
    {
        Destroy(gameObject, life);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Destroy"))
        {
            Destroy(collision.gameObject);
        }
        Destroy(gameObject);
    }
}
