using System;
using Unity.VisualScripting;
using UnityEngine;
using NaughtyAttributes;

public class Enemy : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private Material _visionConeMaterial;
    [SerializeField] private float _visionRange = 10f;
    [SerializeField] private float _visionAngle = 90f;
    [SerializeField] private int _visionConeResolution = 120;
    [SerializeField] private LayerMask _visionObstructingLayer;

    [Header("Shooting")]
    [SerializeField] private bool _hasGun;
    [ShowIf("_hasGun")]
    [SerializeField] private GameObject _bulletPrefab;
    [ShowIf("_hasGun")]
    [SerializeField] private Transform _firePoint;
    [ShowIf("_hasGun")]
    [SerializeField] private float _fireCooldown = 1f;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor = Color.yellow;
    [SerializeField] private Color _alertColor = Color.red;

    private float _visionAngleRad;
    private Mesh _visionConeMesh;
    private MeshFilter _meshFilter;
    private bool _playerSeen = false;
    private float _lastFireTime;

    void Start()
    {
        // Creates a unique material per enemy
        Material _matInstance = new Material(_visionConeMaterial);
        gameObject.AddComponent<MeshRenderer>().material = _matInstance;
        _visionConeMaterial = _matInstance; // Saves the reference to unique material

        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _visionConeMesh = new Mesh();

        _visionAngleRad = _visionAngle * Mathf.Deg2Rad;
    }


    void Update()
    {
        _playerSeen = DetectPlayer();
        if(_hasGun)
        {
            if (_playerSeen && Time.time >= _lastFireTime + _fireCooldown)
            {
                ShootAtPlayer();
                _lastFireTime = Time.time;
            }
        }
        DrawVisionCone();
    }

    public bool DetectPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return false;

        Vector3 _directionToPlayer = player.transform.position - transform.position;
        float _distanceToPlayer = _directionToPlayer.magnitude;

        // Out of range
        if (_distanceToPlayer > _visionRange)
        {
            return false;
        }

        // Check if within angle
        float _angleToPlayer = Vector3.Angle(transform.forward, _directionToPlayer);
        if (_angleToPlayer < _visionAngle / 2f)
        {
            // Check for line of sight
            if (!Physics.Raycast(transform.position, _directionToPlayer.normalized, _distanceToPlayer, _visionObstructingLayer))
            {
                return true;
            }
        }

        return false;
    }

    void DrawVisionCone()
    {
        int[] triangles = new int[(_visionConeResolution - 1) * 3];
        Vector3[] vertices = new Vector3[_visionConeResolution + 1];
        vertices[0] = Vector3.zero;

        float currentAngle = -_visionAngle / 2;
        float angleIncrement = _visionAngle / (_visionConeResolution - 1);

        for (int i = 0; i < _visionConeResolution; i++)
        {
            float angleRad = currentAngle * Mathf.Deg2Rad;
            float sine = Mathf.Sin(angleRad);
            float cosine = Mathf.Cos(angleRad);

            Vector3 raycastDirection = (transform.forward * cosine) + (transform.right * sine);
            Vector3 vertexDirection = (Vector3.forward * cosine) + (Vector3.right * sine);

            if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, _visionRange, ~0))
            {
                vertices[i + 1] = vertexDirection * hit.distance;
            }
            else
            {
                vertices[i + 1] = vertexDirection * _visionRange;
            }

            currentAngle += angleIncrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        _visionConeMesh.Clear();
        _visionConeMesh.vertices = vertices;
        _visionConeMesh.triangles = triangles;
        _meshFilter.mesh = _visionConeMesh;

        UpdateVisionConeColor();
    }

    void UpdateVisionConeColor()
    {
        if (_visionConeMaterial != null)
        {
            _visionConeMaterial.color = _playerSeen ? _alertColor : _defaultColor;
        }
    }

    void ShootAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player == null || _bulletPrefab == null || _firePoint == null) return;

        Vector3 _direction = (player.transform.position - _firePoint.position).normalized;
        GameObject _bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.LookRotation(_direction));

        Rigidbody rb = _bullet.GetComponent<Rigidbody>();

        if( rb != null)
        {
            float _bulletSpeed = 20f;
            rb.linearVelocity = _direction * _bulletSpeed;
        }
    }
}

