using UnityEngine;
using UnityEngine.UI;

public class EndgameAction : MonoBehaviour
{
    [SerializeField] private GameObject _endMenu;
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        _endMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
