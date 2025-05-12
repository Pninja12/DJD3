using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //important
using UnityEngine.SceneManagement;

public enum EnemyState
{
    Idle,
    Patroling,
    FollowingPlayer,
    Dead,
    TakeDamage
}
public class PatrolAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Enemy _vision;
    [SerializeField] private float _range;
    [SerializeField] private float _health = 3;

    [SerializeField] private List<Transform> _points;
    [SerializeField] private float _timeBetweenPatrolPoint;
    //private byte point = 0;
    private EnemyState _state;
    private bool _runCourotineOnce = false;
    private Transform _player;
    private Vector3 _playerPosition;

    [SerializeField] private Animator _anim;
    [SerializeField] private float _aggroRange = 100f;

    private static List<PatrolAI> _enemies = new List<PatrolAI>();


    private int _lastPointIndex = -1;


    void Start()
    {
        if(!_enemies.Contains(this)) _enemies.Add(this);
        if(_anim == null) _anim = GetComponent<Animator>();
        _player = GameObject.Find("Player").transform;
        _state = EnemyState.Idle;
        //point = 0;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

    }



    void Update()
    {
        if (_vision.DetectPlayer())
        {
            _playerPosition = _player.position;
            ChaseMode();

            _anim.ResetTrigger("Patrol");
            _anim.ResetTrigger("turn");
            _anim.ResetTrigger("stopwalk");
        }
        else
        {
            if (_points.Count > 0)
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

                        StopPatrolAndTurn();
                        return;
                    }

                    if (!_runCourotineOnce)
                        StartCoroutine(LeaveTheIdle(_timeBetweenPatrolPoint));
                }
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

    IEnumerator StartWalkingAfterTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartPatrol();
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

    public EnemyState GetState()
    {
        return _state;
    }

    public void Death()
    {
        if (_health > 0)
        {
            _health--;
            ChaseMode();

        }
        else
        {
            _state = EnemyState.Dead;
            Destroy(gameObject);
            _enemies.Remove(this);
        }

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Scene _currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(_currentScene.name);
        }
    }

    private void StartPatrol()
    {
        _anim.ResetTrigger("turn");
        _anim.SetTrigger("Patrol");
    }

    private void StopPatrolAndTurn()
    {
        _anim.ResetTrigger("Patrol");
        _anim.SetTrigger("turn");
        StartCoroutine(StartWalkingAfterTurn(0.5f));
    }

    public void ChaseMode(bool propagate = true)
    {
        if (_state == EnemyState.FollowingPlayer) return;

        _state = EnemyState.FollowingPlayer;
        _agent.SetDestination(_player.position);

        _anim.ResetTrigger("Patrol");
        _anim.ResetTrigger("turn");
        _anim.ResetTrigger("stopwalk");

        if (!propagate) return;

        foreach (PatrolAI enemy in _enemies)
        {
            if (enemy != this &&
                enemy != null &&
                Vector3.Distance(transform.position, enemy.transform.position) <= _aggroRange &&
                enemy._state != EnemyState.FollowingPlayer)
            {
                enemy.ChaseMode(false);
            }
        }
    }

    public void InvestigatePosition(Vector3 position)
    {
        if(_state == EnemyState.Dead || _state == EnemyState.FollowingPlayer) return;

        _state = EnemyState.FollowingPlayer;
        _agent.SetDestination(position);

        _anim.ResetTrigger("Patrol");
        _anim.ResetTrigger("turn");
        _anim.ResetTrigger("stopwalk");
    }
    
}