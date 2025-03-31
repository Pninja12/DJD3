using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform _bulletSpawn;
    public GameObject _bulletPrefab;
    public float _bulletSpeed = 10;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var bullet = Instantiate(_bulletPrefab, _bulletSpawn.position, _bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = _bulletSpawn.forward * _bulletSpeed;
        }
    }
}
