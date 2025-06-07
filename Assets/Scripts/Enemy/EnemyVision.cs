using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float _visionRange = 10f;
    [SerializeField] private float _visionAngle = 120f;
    [SerializeField] private LayerMask _visionObstructingLayer;
    [SerializeField] private Transform _eyes;

    private Transform _playerTransform;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Problem!");
        }
    }

    public bool DetectPlayer()
    {
        if (_playerTransform == null) return false;

        Vector3 eyePosition = _eyes.position;
    
        Vector3 playerPosition = _playerTransform.position + Vector3.up * 1.0f;

        Vector3 directionToPlayer = playerPosition - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > _visionRange)
            return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > _visionAngle / 2f)
            return false;

    
        if (!Physics.Raycast(eyePosition, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer, _visionObstructingLayer))
        {
            Debug.DrawRay(eyePosition, directionToPlayer.normalized * distanceToPlayer, Color.green);
            return true;
        }

        Debug.DrawRay(eyePosition, directionToPlayer.normalized * hit.distance, Color.red);
        return false;
    }

}
