using System;
using UnityEngine;
using NaughtyAttributes;

public class Enemy : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float _visionRange = 10f;
    [SerializeField] private float _visionAngle = 90f;
    [SerializeField] private LayerMask _visionObstructingLayer;

    [Header("Shooting")]
    [SerializeField] private bool _hasGun;
    [ShowIf("_hasGun")]
    [SerializeField] private GameObject _bulletPrefab;
    [ShowIf("_hasGun")]
    [SerializeField] private Transform _firePoint;
    [ShowIf("_hasGun")]
    [SerializeField] private float _fireCooldown = 1f;

    private bool _playerSeen = false;
    private float _lastFireTime;

    void Update()
    {
        _playerSeen = DetectPlayer();
        if (_hasGun && _playerSeen && Time.time >= _lastFireTime + _fireCooldown)
        {
            ShootAtPlayer();
            _lastFireTime = Time.time;
        }
    }

    public bool DetectPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return false;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > _visionRange)
            return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer < _visionAngle / 2f)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, _visionObstructingLayer))
            {
                print("See player");
                return true;
            }
        }

        return false;
    }

    void ShootAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || _bulletPrefab == null || _firePoint == null) return;

        Vector3 direction = (player.transform.position - _firePoint.position).normalized;
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float bulletSpeed = 20f;
            rb.linearVelocity = direction * bulletSpeed;
        }
    }
}
