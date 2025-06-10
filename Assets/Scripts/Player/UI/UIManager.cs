using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Codigo feito pelo carvalho neste script precisa de ser revisto por programadores.
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _deadMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _crossHair;
    private Button _resumeButton;

    private bool _openPauseMenu;
    //Add pelo carvalho
    private bool _opendeadMenu;
    //
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _openPauseMenu = false;
        //Add pelo carvalho
        _opendeadMenu = false;
        
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
        //ADD pelo carvalho
        if (_opendeadMenu)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
        }
        //


    }

    public bool GetPause()
    {
        return (_openPauseMenu || _deadMenu.activeSelf);
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
        _opendeadMenu = true;

    }
    //
}
