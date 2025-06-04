using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _crossHair;
    private Button _resumeButton;

    private bool _openPauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _openPauseMenu = false;
        _crossHair.SetActive(false);
        _pauseMenu.SetActive(false);
        Transform buttonTransform = transform.Find("Pause Menu/ResumeButton");
        _resumeButton = buttonTransform.GetComponent<Button>();
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
            _openPauseMenu = !_openPauseMenu;
        }


        if (_openPauseMenu)
        {
            Time.timeScale = 0f;
            _pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            _resumeButton.onClick.AddListener(TurnOff);

        }

        else
        {
            _pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }


    }

    public bool GetPause()
    {
        return _openPauseMenu;
    }

    void TurnOff()
    {
        _openPauseMenu = false;
    }
}
