using UnityEngine;
using UnityEngine.UI;

public class EndgameAction : MonoBehaviour
{
    [SerializeField] private GameObject _endMenu;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            _endMenu.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            _endMenu.SetActive(true);
    }
}
