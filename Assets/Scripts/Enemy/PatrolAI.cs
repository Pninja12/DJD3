using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //important


public class PatrolAI : MonoBehaviour 
{
    public NavMeshAgent _agent;
    public float _range; 

    public Transform _centrePoint; 
    

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        if(_agent.remainingDistance <= _agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(_centrePoint.position, _range, out point)) //pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                _agent.SetDestination(point);
            }
        }

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