using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI; //important
using UnityEngine.SceneManagement;

public enum EnemyState
{
    Idle,
    Patrolling,
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

    [SerializeField] private float _chaseSpeed = 10;
    [SerializeField] private Detected _detected;

    [SerializeField] private List<Transform> _points;
    [SerializeField] private float _timeBetweenPatrolPoint;
    //private byte point = 0;
    private EnemyState _state;
    private bool _runCourotineOnce = false;
    private bool _hasPlayedDetectionSound = false;

    private Transform _player;
    private Vector3 _playerPosition;

    [SerializeField] private Animator _anim;
    [SerializeField] private float _aggroRange = 100f;

    private bool _isAttacking = false;

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
        if (_state != EnemyState.Dead)
        {
            _anim.SetBool("Patrol", _state == EnemyState.Patrolling);

            _anim.SetBool("FoundPlayer", _state == EnemyState.FollowingPlayer);

            _anim.SetBool("GoIdle", _state == EnemyState.Idle);


            
            if (_vision.DetectPlayer())
            {
                _playerPosition = _player.position;

                if (!_hasPlayedDetectionSound && _detected != null)
                {
                    _detected.Detect();
                    _hasPlayedDetectionSound = true;
                }

                ChaseMode(_player.position);
            }
            else if (Vector3.Distance(transform.position, _playerPosition) < 3)
            {
                _playerPosition = _player.position;
                AttackMode();
            }
            else
            {
                if (_points.Count > 0 && _points != null)
                {
                    _playerPosition = Vector3.zero;

                    if (!(_agent.hasPath || _agent.pathPending) && _agent.velocity.sqrMagnitude < 0.1f)
                    {
                        //_anim.SetTrigger("PatrolToStop");
                        if (_state == EnemyState.Idle && !_runCourotineOnce)
                        {
                            int randomIndex = Random.Range(0, _points.Count);

                            while (_points.Count > 1 && randomIndex == _lastPointIndex)
                            {
                                randomIndex = Random.Range(0, _points.Count);
                            }

                            _lastPointIndex = randomIndex;
                            _agent.SetDestination(_points[randomIndex].position);
                            _state = EnemyState.Patrolling;
                            return;

                        }

                        if (!_runCourotineOnce)
                            StartCoroutine(LeaveTheIdle(_timeBetweenPatrolPoint));

                    }
                }
                else
                    _state = EnemyState.Idle;
            } 
        }
    }

    IEnumerator LeaveTheIdle(float timeToWait)
    {
        _runCourotineOnce = true;
        _state = EnemyState.Idle;
        yield return new WaitForSeconds(timeToWait);
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

    public EnemyState GetState()
    {
        return _state;
    }

    public void Death(int damage = 1)
    {
        if (damage >= _health && _state != EnemyState.Dead)
        {
            _state = EnemyState.Dead;

            _agent.ResetPath();
            _agent.isStopped = true;
            _agent.enabled = false;
            _anim.SetTrigger("Death");
            StartCoroutine(DeathSequence());
        }
        else if(_state != EnemyState.Dead)
        {
            _health = _health - damage;
            _anim.SetTrigger("HitFront");
            StartCoroutine(WaitForReset());
        }

    }

    private IEnumerator DeathSequence()
    {
        _enemies.Remove(this);

        yield return new WaitForSeconds(5f);

        Destroy(gameObject);
    }

    private IEnumerator WaitForReset()
    {
        _state = EnemyState.TakeDamage;

        yield return new WaitForSeconds(1f);

        _playerPosition = _player.position;
        ChaseMode(_player.position);

    }

    /*void OnTriggerEnter(Collider collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            print(collision.gameObject.layer);
            if (gameObject.layer == LayerMask.NameToLayer("Enemy") && collision.bounds.Intersects(GetComponent<Collider>().bounds))
            {
                Scene _currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(_currentScene.name);
            }

        }
    }*/


    public void ChaseMode(Vector3 playerPos, bool propagate = true)
    {
        //if (_state == EnemyState.FollowingPlayer) return;

        _playerPosition = playerPos;
        _state = EnemyState.FollowingPlayer;
        _agent.SetDestination(_playerPosition);
        _agent.speed = _chaseSpeed;

        

        if (!propagate) return;

        foreach (PatrolAI enemy in _enemies)
        {
            if (enemy != this &&
                enemy != null &&
                Vector3.Distance(transform.position, enemy.transform.position) <= _aggroRange &&
                enemy._state != EnemyState.FollowingPlayer)
            {
                enemy.ChaseMode(playerPos, false);
            }
        }
    }

    private void AttackMode()
    {
        if(_isAttacking) return;

        _state = EnemyState.FollowingPlayer;

        _agent.isStopped = true;

        

        //_agent.speed = _chaseSpeed;
        _anim.SetTrigger("Attack" + Random.Range(1,3));

        StartCoroutine(AttackCooldown(1.2f));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterHit>()?.TakeHit();
        }
    }

    private IEnumerator AttackCooldown(float _duration)
    {
        yield return new WaitForSeconds(_duration);

        _agent.isStopped = false;
        _isAttacking = false;
    }

    public void InvestigatePosition(Vector3 position)
    {
        if (_state == EnemyState.Dead || _state == EnemyState.FollowingPlayer) return;

        _state = EnemyState.FollowingPlayer;
        _agent.SetDestination(position);
        _anim.SetBool("Patrol", true);

        
    }
    
}