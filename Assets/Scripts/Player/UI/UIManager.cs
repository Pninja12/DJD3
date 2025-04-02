using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _crossHair;

    private bool _openPauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _openPauseMenu = false;
        _crossHair.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Camera") && !_openPauseMenu)
        {
            _crossHair.SetActive(true);
        }
        else
            _crossHair.SetActive(false);


        if (Input.GetButtonDown("Cancel"))
        {
            _pauseMenu.SetActive(!_pauseMenu.activeSelf);
        }
        
    }
}
