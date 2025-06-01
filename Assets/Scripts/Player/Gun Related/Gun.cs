using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform _bulletSpawn;
    public GameObject _bulletPrefab;
    public float _bulletSpeed = 10;

    [SerializeField] private int _maxAmmo = 3;
    private int _currentAmmo;

    void Start()
    {
        _currentAmmo = _maxAmmo;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && _currentAmmo > 0)
        {
            var bullet = Instantiate(_bulletPrefab, _bulletSpawn.position, _bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = _bulletSpawn.forward * _bulletSpeed;

            _currentAmmo--;
        }
    }

    public void AddAmo(int amount)
    {
        _currentAmmo += amount;
        if(_currentAmmo > _maxAmmo) _currentAmmo = _maxAmmo;
    }
}
