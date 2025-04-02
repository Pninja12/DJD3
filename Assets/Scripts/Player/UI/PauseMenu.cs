using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private GameObject _exitButton;
    private bool _exit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _exitButton = transform.Find("Exit").gameObject;
        _exit = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ExitClicked()
    {
        return _exit;
    }
}
