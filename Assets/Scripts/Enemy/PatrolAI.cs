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
    [SerializeField] private float _range; 

    [SerializeField] private List<Transform> _points;
    [SerializeField] private float _timeBetweenPatrolPoint; 
    private byte point = 0;
    private EnemyState _state;
    private bool _runCourotineOnce = false;


    void Start()
    {
        _state = EnemyState.Idle;
        point = 0;
        
    }


    void Update()
    {
        if (point >= _points.Count)
        {
            point = 0;
        }

        //print($"_agent.pathPending - {_agent.pathPending}\n_agent.remainingDistance - {_agent.remainingDistance}\n_agent.stoppingDistance - {_agent.stoppingDistance}\n_agent.hasPath - {_agent.hasPath}\n_agent.velocity.sqrMagnitude - {_agent.velocity.sqrMagnitude}");
        //print($"_agent.hasPath - {_agent.hasPath}\n \n_agent.velocity.sqrMagnitude - {_agent.velocity.sqrMagnitude}");
        if (!(_agent.hasPath || _agent.pathPending) && _agent.velocity.sqrMagnitude < 0.1)
        {
            //print(_state);
            if (_state == EnemyState.Idle && _points.Count > 2)
            {
                _agent.SetDestination(_points[point].position);
                _state = EnemyState.Patroling;
                //print("Is now patroling");
                point++;
                return;
            }
            if(!_runCourotineOnce)
                StartCoroutine(LeaveTheIdle(_timeBetweenPatrolPoint));
            
        }

        /* if(_agent.remainingDistance <= _agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(_centrePoint.position, _range, out point)) //pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                _agent.SetDestination(point);
            }
        } */

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