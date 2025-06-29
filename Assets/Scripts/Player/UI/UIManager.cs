using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
// Codigo feito pelo carvalho neste script precisa de ser revisto por programadores.
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _deadMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _endMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _crossHair;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private AudioMixer AudioMixer;
    [SerializeField] private Light sceneLight;
    [SerializeField] private Volume globalVolume;
    //private Button _resumeButton;

    private bool _openPauseMenu;
    private bool _openDeadMenu;
    private bool _openEndMenu;
    private  bool _openSettingsMenu;

    private ColorAdjustments colorAdjustments;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _openPauseMenu = false;
        _openDeadMenu = false;
        _openEndMenu = false;
        _openSettingsMenu = false;

        //
        _crossHair.SetActive(false);
        _pauseMenu.SetActive(false);
        _deadMenu.SetActive(false);
        _endMenu.SetActive(false);
        _settingsMenu.SetActive(false);
        //Transform buttonTransform = transform.Find("Pause Menu/ResumeButton");
        //_resumeButton = buttonTransform.GetComponent<Button>();

        if (globalVolume != null && globalVolume.profile != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            // Optionally initialize here
            colorAdjustments.postExposure.value = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _openDeadMenu = _deadMenu.activeSelf;
        _openEndMenu = _endMenu.activeSelf;
        _settingsMenu.SetActive(_openSettingsMenu);
        _pauseMenu.SetActive(_openPauseMenu);

        if (Input.GetButton("Camera") && !_openPauseMenu)
        {
            _crossHair.SetActive(true);
        }
        else
            _crossHair.SetActive(false);


        if (Input.GetButtonDown("Cancel") && !_deadMenu.activeSelf && !_endMenu.activeSelf && !_settingsMenu.activeSelf)
        {
            _openPauseMenu = !_openPauseMenu;
        }
        else if (Input.GetButtonDown("Cancel") && _settingsMenu.activeSelf)
        {
            SettingsOff();
        }


        if (_openPauseMenu || _openDeadMenu || _openEndMenu || _openSettingsMenu)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
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
        return (_openPauseMenu || _openDeadMenu || _openEndMenu || _openSettingsMenu);
    }

    public void TurnOff()
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

    public void SettingsOn()
    {
        _openPauseMenu = false;
        _openSettingsMenu = true;
    }

    public void SettingsOff()
    {
        _openPauseMenu = true;
        _openSettingsMenu = false;
    }

    public void SetVolume(float volume)
    {
        AudioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetBrightness(float value)
    {
        colorAdjustments.postExposure.value = value;
    }
}
