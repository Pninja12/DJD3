using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet : MonoBehaviour
{
   private float _life = 3f;
   

    void Awake()
    {
        Destroy(gameObject, _life);
    }

    void OnTriggerEnter(Collider collision)
    {
        string tag = collision.gameObject.tag;

        switch (tag)
        {
            case "Destroy":
                var _destructible = collision.gameObject.GetComponent<Distract>();
                if(_destructible != null)
                {
                    _destructible.DestroyObject();
                }
                else
                {
                    Destroy(collision.gameObject);
                }
                Destroy(gameObject);
                break;
            case "Player":
                RestartLevel();
                Destroy(gameObject);
                break;
            case "Enemy":
                PatrolAI enemy = collision.gameObject.GetComponentInParent<PatrolAI>();
                enemy.Death();
                Destroy(gameObject);
                break;
        }
    }

    void RestartLevel()
    {
        Scene _currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(_currentScene.name);
    }
}
