using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Gun : MonoBehaviour
{
    //Add pelo carvalho
    public TextMeshProUGUI TextBulletsUI;
    //
    public Transform _bulletSpawn;
    public GameObject _bulletPrefab;
    public float _bulletSpeed = 10;

    [SerializeField] private int _maxAmmo = 3;
    private int _currentAmmo;
    [SerializeField] private UIManager ui;

    void Start()
    {
        _currentAmmo = _maxAmmo;
        UIAmmo();
    }
    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(0) && _currentAmmo > 0) && !ui.GetPause())
        {
            var bullet = Instantiate(_bulletPrefab, _bulletSpawn.position, _bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = _bulletSpawn.forward * _bulletSpeed;

            _currentAmmo--;
            UIAmmo();
        }
    }

    public void AddAmo(int amount)
    {
        _currentAmmo += amount;
        if (_currentAmmo > _maxAmmo) _currentAmmo = _maxAmmo;
        UIAmmo();
    }
    public void UIAmmo()
    {
        //Add pelo carvalho
        TextBulletsUI.text = _currentAmmo.ToString();
        //

    }
}
