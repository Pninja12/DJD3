using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //important

enum EnemyState
{
    Idle,
    Patroling,
    FollowingPlayer,
    Dead
}
public class PatrolAI : MonoBehaviour 
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Enemy _vision;
    [SerializeField] private float _range; 

    [SerializeField] private List<Transform> _points;
    [SerializeField] private float _timeBetweenPatrolPoint; 
    private byte point = 0;
    private EnemyState _state;
    private bool _runCourotineOnce = false;
    private Transform _player;
    private Vector3 _playerPosition;

    private int _lastPointIndex = -1;


    void Start()
    {
        _player = GameObject.Find("Player").transform;
        _state = EnemyState.Idle;
        point = 0;
        
    }


    
    void Update()
    {
        if (_vision.DetectPlayer())
        {
            _playerPosition = _player.position;
            _agent.SetDestination(_playerPosition);
            _state = EnemyState.FollowingPlayer;
        }
        else
        {
            _playerPosition = Vector3.zero;

            if (!(_agent.hasPath || _agent.pathPending) && _agent.velocity.sqrMagnitude < 0.1f)
            {
                if (_state == EnemyState.Idle)
                {
                    int randomIndex = Random.Range(0, _points.Count);

                    // Evita repetir o mesmo ponto duas vezes seguidas
                    while (_points.Count > 1 && randomIndex == _lastPointIndex)
                    {
                        randomIndex = Random.Range(0, _points.Count);
                    }

                    _lastPointIndex = randomIndex;
                    _agent.SetDestination(_points[randomIndex].position);

                    _state = EnemyState.Patroling;
                    return;
                }

                if (!_runCourotineOnce)
                    StartCoroutine(LeaveTheIdle(_timeBetweenPatrolPoint));
            }
        }
    }

    IEnumerator LeaveTheIdle(float timeToWait)
    {
        _runCourotineOnce = true;
        yield return new WaitForSeconds(timeToWait);
        _state = EnemyState.Idle;
        //print("Is now Idle");
        _runCourotineOnce = false;
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    
}