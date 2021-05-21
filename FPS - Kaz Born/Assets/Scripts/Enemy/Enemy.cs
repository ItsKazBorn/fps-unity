using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("Health")] [SerializeField] private float health = 100f;

    [Header("Field of View")] 
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private float seeRange = 5f;
    [SerializeField] private Transform _player;
    
    [Header("Gun")]
    [SerializeField] private EnemyGun _gun;

    [Header("Patrol Points")] 
    [SerializeField] private List<PatrolPoint> _patrolPoints;
    private int _currentPatrolIndex = 0;
    [SerializeField] private float _maxWaitTime = 6f;
    [SerializeField] private float _minWaitTime = 3f;
    private float _waitTime = 5f;
    private bool _patrolForward = true;
    
    private Collider[] overlaps = new Collider[50];
    
    private NavMeshAgent _navMeshAgent;
    private Transform _transform;
    
    private Quaternion _initialRotation;
    private Vector3 _initialPosition;
    private Vector3 _lastPlayerPosition;

    private bool isGameOver = false;

    private bool _rotateToInitialRotation;
    private bool _isInShootingRange;
    public bool IsInShootingRange
    {
        get { return _isInShootingRange; }
        set
        {
            if (value == _isInShootingRange)
                return;

            _isInShootingRange = value;
            if (!_isInShootingRange)
            {
                // execute code
                _rotateToInitialRotation = false;
                StopAllCoroutines();
                StartCoroutine(GoToLastPlayerPosition());
            }

            if (_isInShootingRange)
            {
                //_rotateToInitialRotation = false;
                _navMeshAgent.ResetPath();
                StopAllCoroutines(); 
            }
        }
    }

    private bool _isInSeeRange;

    void Start()
    {
        _transform = transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _initialPosition = _transform.position;
        _initialRotation = _transform.rotation;
        _player = GameObject.Find("Player").transform;
        _lastPlayerPosition = _initialPosition;
        GameEvents.current.onGameOver += OnGameOver;
        ChangePatrolPoint();
    }

    private void OnDestroy()
    {
        GameEvents.current.onGameOver -= OnGameOver;
    }

    void Update()
    {
        //CheckIfPlayerIsInShootingDistance();
        if (!isGameOver)
        {
            IsInShootingRange = InFOV(_transform, _player, maxAngle, shootingRange);
            _isInSeeRange = InFOV(_transform, _player, maxAngle, seeRange);
            GetPlayerPosition();
            ShootPlayer();
            //RotateToInitialRotation();
        }
    }

    void OnGameOver()
    {
        _navMeshAgent.ResetPath();
        isGameOver = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void ShootPlayer()
    {
        if (_isInShootingRange)
        {
            TurnToPlayer();
            _gun.Fire();
        } 
        else 
            _gun.StopFiring();
    }

    void TurnToPlayer()
    {
        _rotateToInitialRotation = false;
        Vector3 targetDirection = (_player.position - _transform.position).normalized;
        targetDirection.y *= 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, 10);
    }
    
    float CheckPlayerDistance()
    {
        return Vector3.Distance(_player.transform.position, _transform.position);
    }

    void GetPlayerPosition()
    {
        if (_isInSeeRange)
        {
            _lastPlayerPosition = _player.transform.position;
        }
    }
    
    void ChangePatrolPoint()
    {
        Debug.Log("Changing Patrol Point");
        if (_patrolForward)
        {
            _currentPatrolIndex++;
            if (_currentPatrolIndex >= _patrolPoints.Count)
                _currentPatrolIndex = 0;
        }
        else
        {
            _currentPatrolIndex--;
            if (_currentPatrolIndex < 0)
                _currentPatrolIndex = _patrolPoints.Count - 1;
        }
        StartCoroutine(TravelBetweenPoints());
    }

    IEnumerator TravelBetweenPoints()
    {
        GoTo(_patrolPoints[_currentPatrolIndex].transform.position);

        // Wait for Path to be over
        yield return new WaitForSeconds(0.5f);
        while (_navMeshAgent.remainingDistance >= 0.1f)
        {
            yield return null;
        }

        // If it's initial Point
        if (_currentPatrolIndex == 0)
        {
            // Wait a few seconds
            _waitTime = Random.Range(_minWaitTime, _maxWaitTime);
            yield return new WaitForSeconds(_waitTime);
            // Randomize patrol direction
            if (Random.Range(0f, 1f) <= 0.5f)
            {
                _patrolForward = !_patrolForward;
            }
        }
        ChangePatrolPoint();
    }

    IEnumerator GoToLastPlayerPosition()
    {
        Debug.Log("Going to Last Player Position");
        _navMeshAgent.ResetPath();
        if (_lastPlayerPosition != _initialPosition)
        {
            GoTo(_lastPlayerPosition);
        
            // Wait for path to be over
            yield return new WaitForSeconds(0.5f);
            while (_navMeshAgent.remainingDistance >= 0.1f)
            {
                yield return null;
            }
        }

        Debug.Log("Waiting a bit");
        // Wait a few seconds at destination
        yield return new WaitForSeconds(5f);

        StartCoroutine(SearchForPlayer());
    }

    IEnumerator SearchForPlayer()
    {
        Debug.Log("Searching for player");
        Vector3 direction1 = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        Vector3 direction2 = new Vector3(-direction1.x, 0f, -direction1.z);

        Vector3 position1 = _transform.TransformPoint(direction1);
        Vector3 position2 = _transform.TransformPoint(direction2);
        
        Debug.Log("Going to Pos 1");
        GoTo(position1);
        // Wait for path to be over
        yield return new WaitForSeconds(0.5f);
        while (_navMeshAgent.remainingDistance >= 0.1f)
        {
            yield return null;
        }

        Debug.Log("Waiting a bit");
        yield return new WaitForSeconds(1f);
        
        Debug.Log("Going to Pos 2");
        GoTo(position2);
        // Wait for path to be over
        yield return new WaitForSeconds(0.5f);
        while (_navMeshAgent.remainingDistance >= 0.1f)
        {
            yield return null;
        }

        Debug.Log("Waiting a bit");
        yield return new WaitForSeconds(1f);

        _lastPlayerPosition = _initialPosition;
        Debug.Log("Resume normal Path");
        StartCoroutine(TravelBetweenPoints());
    }

    void GoTo(Vector3 destination)
    {
        if (_navMeshAgent)
        {
            Vector3 targetVector = destination;
            _navMeshAgent.SetDestination(targetVector);
        }
    }

    bool InFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overlaps);

        for (int i = 0; i < count + 1; i++)
        {
            if (overlaps[i])
            {
                if (overlaps[i].transform == target)
                {
                    Vector3 directionBetween = (target.position - checkingObject.position).normalized;
                    directionBetween.y *= 0;

                    float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                    if (angle <= maxAngle)
                    {
                        Vector3 direction = (target.position - checkingObject.position);
                        RaycastHit hit;
                        if (Physics.Raycast(checkingObject.position, direction, out hit, maxRadius))
                        {
                            if (hit.transform == target)
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, seeRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * shootingRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * shootingRange;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * shootingRange);
        
        if (!IsInShootingRange)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Vector3 direction = (_player.transform.position - transform.position).normalized * shootingRange;
        direction.y *= 0;
        Gizmos.DrawRay(transform.position, direction);
    }
    
    IEnumerator GoToInitialPosition()
    {
        //_rotateToInitialRotation = false;
        yield return new WaitForSeconds(0.5f);
        while (_navMeshAgent.remainingDistance >= 0.1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        GoTo(_patrolPoints[_currentPatrolIndex].transform.position);
        //GoBackToPatrol();
        /*
        yield return new WaitForSeconds(0.5f);
        while (_navMeshAgent.remainingDistance >= 0.1f)
        {
            yield return null;
        }

        _rotateToInitialRotation = true;
        */
    }

    
    void CheckIfPlayerIsInShootingDistance()
    {
        float distance = CheckPlayerDistance();

        if (distance <= shootingRange)
        {
            IsInShootingRange = true;
        }
        else
            IsInShootingRange = false;
    }
    
    void RotateToInitialRotation()
    {
        if (_rotateToInitialRotation)
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, _initialRotation, 2);
    }
}
