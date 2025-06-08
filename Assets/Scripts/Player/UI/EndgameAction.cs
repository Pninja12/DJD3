using UnityEngine;
using UnityEngine.UI;

public class EndgameAction : MonoBehaviour
{
    [SerializeField] private GameObject _endMenu;
    public void EndPanel()
    {
        _endMenu.SetActive(true);
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        _endMenu.SetActive(true);
    }
}
