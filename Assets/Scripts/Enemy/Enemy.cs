using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Material _visionConeMaterial;
    [SerializeField] private float _visionRange;
    [SerializeField] private float _visionAngle;
    [SerializeField] private LayerMask _visionObstructingLayer;
    [SerializeField] private int _visionConeResolution = 120;
    private float _visionAngleRad; 
    Mesh _visionConeMesh;
    MeshFilter _meshFilter;

    void Start()
    {
        transform.AddComponent<MeshRenderer>().material = _visionConeMaterial;
        _meshFilter = transform.AddComponent<MeshFilter>();
        _visionConeMesh = new Mesh();

        _visionAngleRad = _visionAngle * Mathf.Deg2Rad;
    }

    void Update()
    {
        DrawVisionCone();
    }

    void DrawVisionCone()
    {
	    int[] triangles = new int[(_visionConeResolution - 1) * 3];
    	Vector3[] Vertices = new Vector3[_visionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float _currentangle = -_visionAngle / 2;
        float _angleIcrement = _visionAngle / (_visionConeResolution - 1);
        float _sine;
        float _cosine;
        

        for (int i = 0; i < _visionConeResolution; i++)
        {
            _sine = Mathf.Sin(_currentangle);
            _cosine = Mathf.Cos(_currentangle);
            Vector3 RaycastDirection = (transform.forward * _cosine) + (transform.right * _sine);
            Vector3 VertForward = (Vector3.forward * _cosine) + (Vector3.right * _sine);
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, _visionRange, _visionObstructingLayer))
            {
                Vertices[i + 1] = VertForward * hit.distance;

            }
            else
            {
                Vertices[i + 1] = VertForward * _visionRange;
            }

            _currentangle += _angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        _visionConeMesh.Clear();
        _visionConeMesh.vertices = Vertices;
        _visionConeMesh.triangles = triangles;
        _meshFilter.mesh = _visionConeMesh;
    }
}
