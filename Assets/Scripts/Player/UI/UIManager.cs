using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Codigo feito pelo carvalho neste script precisa de ser revisto por programadores.
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _deadMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _endMenu;
    [SerializeField] private GameObject _crossHair;
    [SerializeField] private Slider _staminaSlider;
    private Button _resumeButton;

    private bool _openPauseMenu;
    private bool _openDeadMenu;
    private bool _openEndMenu;
    //
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _openPauseMenu = false;
        _openDeadMenu = false;
        
        //
        _crossHair.SetActive(false);
        _pauseMenu.SetActive(false);
        _deadMenu.SetActive(false);
        Transform buttonTransform = transform.Find("Pause Menu/ResumeButton");
        _resumeButton = buttonTransform.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

        _openDeadMenu = _deadMenu.activeSelf;
        _openEndMenu = _endMenu.activeSelf;

        if (Input.GetButton("Camera") && !_openPauseMenu)
        {
            _crossHair.SetActive(true);
        }
        else
            _crossHair.SetActive(false);


        if (Input.GetButtonDown("Cancel") && !_deadMenu.activeSelf)
        {
            _openPauseMenu = !_openPauseMenu;
        }


        if (_openPauseMenu && !_openDeadMenu && !_openEndMenu)
        {
            Time.timeScale = 0f;
            _pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            _resumeButton.onClick.AddListener(TurnOff);

        }
        else if (_openDeadMenu || _openEndMenu)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            _pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        //


    }
    public void UpdateStaminaBar(float current, float max)
    {
        _staminaSlider.value = current / max;
    }

    public bool GetPause()
    {
        return (_openPauseMenu || _openDeadMenu || _openEndMenu);
    }

    void TurnOff()
    {
        _openPauseMenu = false;
    }
    //Add pelo carvalho
    public void DeadPanel()
    {
        StartCoroutine(WaitForDeadPanel());
    }
    
    private IEnumerator WaitForDeadPanel()
    {
        // espera Xs
        yield return new WaitForSeconds(3f);
        _deadMenu.SetActive(true);
        _openDeadMenu = true;

    }
    //
}
