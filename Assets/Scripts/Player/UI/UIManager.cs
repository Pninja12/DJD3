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


        if (Input.GetButton("Cancel"))
        {
            _pauseMenu.SetActive(true);
        }

        if (_pauseMenu.GetComponent<PauseMenu>().ExitClicked())
        {
            _pauseMenu.SetActive(false);
        }
        
    }
}
