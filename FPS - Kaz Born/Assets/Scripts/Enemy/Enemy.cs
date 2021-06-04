using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.GameCenter;
using Random = UnityEngine.Random;

public enum EnemyState
 {
     Waiting, Patrolling, Chasing, Attacking
 }

public class Enemy : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Transform _transform;
    private EnemyState state;
    
    [Header("Health")] 
    [SerializeField] private float health = 100f;
    private bool isGameOver = false;

    [Header("Field of View")] 
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private float seeRange = 5f;
    [SerializeField] private Transform player;
    private Collider[] overlaps = new Collider[50];
    private Vector3 _lastPlayerPosition;
    private Vector3 _nullPosition;
    private bool _lockChasing;
    
    [Header("Gun")]
    [SerializeField] private EnemyGun _gun;

    [Header("Patrol")] 
    [SerializeField] private LayerMask _patrolPointsLayerMask;
    private List<Collider> _patrolPoints = new List<Collider>();
    private List<int> _allRouteIndexes;
    private List<int> _currentRoute = new List<int>();
    private List<int> _alertRoute = new List<int>();
    private List<int> _normalRoute = new List<int>();
    private Vector3 _currentDestination;
    private bool _patrolling;
    private bool _alerted;
    
    
    [SerializeField] private float _minWaitTime = 3f;
    [SerializeField] private float _maxWaitTime = 6f;
    private float _waitTime = 5f;

    private bool _isInShootingRange;
    private bool _isInSeeRange;

    public bool IsInSeeRange
    {
        set
        {
            if (value == _isInSeeRange)
                return;

            _isInSeeRange = value;
            if (!_isInSeeRange)
            {
                Debug.Log("Setting to chase");
                SetState(EnemyState.Chasing);
            }
        }
    }


    // ------------------- SET UP
    #region SetUp
    void Start()
    {
        _transform = transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _nullPosition = new Vector3(0,0,0);
        _lastPlayerPosition = _nullPosition;
        player = GameObject.Find("Player").transform;
        GameEvents.current.onGameOver += OnGameOver;
        StartPatrol();
    }

    private void OnDestroy()
    {
        GameEvents.current.onGameOver -= OnGameOver;
    }

    private void Awake()
    {
         Collider[] colliders = new Collider[30];
        // _patrolPoints = new Collider[25];
        
        
        
        int numberOfPoints = Physics.OverlapSphereNonAlloc(Vector3.zero, 100, colliders, _patrolPointsLayerMask);

        foreach (var collider in colliders.Take(numberOfPoints)) 
        { 
            _patrolPoints.Add(collider);
        }
        
        _allRouteIndexes = new List<int>();
        for (int i = 0; i < _patrolPoints.Count - 1; i++)
        {
            _allRouteIndexes.Add(i);
        }
        _currentRoute.Clear();
    }

    void Update()
    {
        if (!isGameOver)
        {
            switch (state)
            {
                case EnemyState.Waiting:
                    _lockChasing = false;
                    
                    break;
                case EnemyState.Patrolling:
                    _lockChasing = false;
                    Patrol();
                    break;
                case EnemyState.Chasing:
                    if (!_lockChasing)
                    {
                        StopAllCoroutines();
                        StartCoroutine(GoToLastPlayerPosition());
                    }
                    break;
                case EnemyState.Attacking:
                    _lockChasing = false;
                    Attack();
                    break;
            }
            
            _isInShootingRange = InFOV(_transform, player, maxAngle, shootingRange);
            IsInSeeRange = InFOV(_transform, player, maxAngle, seeRange);
            if (_isInShootingRange)
            {
                SetState(EnemyState.Attacking);
            }
            if (_isInSeeRange)
            {
                GetPlayerPosition();
            }
        }
    }

    void OnGameOver()
    {
        _navMeshAgent.ResetPath();
        isGameOver = true;
    }
    
    #endregion
    
    // ------------------- LIFE
    #region Life

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
    #endregion
    
    // ------------------- ATTACK
    #region Attack

    void Attack()
    {
        StopAllCoroutines();
        if (_navMeshAgent.hasPath)
        {
            _navMeshAgent.ResetPath();
        }
        ShootPlayer();
    }
    
    void ShootPlayer()
    {
        if (_isInShootingRange)
        {
            TurnToPlayer();
            _gun.Fire();
        }
        else
        {
            _gun.StopFiring();
        }
    }

    void TurnToPlayer()
    {
        Vector3 targetDirection = (player.position - _transform.position).normalized;
        targetDirection.y *= 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, 10);
    }
    #endregion
    
    // ------------------- PATROL
    #region Patrol
    void Patrol()
    {
        if (_patrolling && _navMeshAgent.remainingDistance <= 0.5f)
        {
            StartCoroutine(TraverseRoute(_currentRoute));
        }
    }

    void StartPatrol()
    {
        if (_patrolPoints.Count >= 2)
        {
            SetState(EnemyState.Patrolling);
            StartCoroutine(TraverseRoute(_currentRoute));
        }
    }

    IEnumerator TraverseRoute(List<int> route)
    {
        _patrolling = false;
        
        if (route.Count <= 0)
        {
            _waitTime = Random.Range(_minWaitTime, _maxWaitTime);
            yield return new WaitForSeconds(_waitTime);
            Debug.Log("New Route");
            SetRoute();
        }
        _navMeshAgent.SetDestination(GetNextDestination(route));
        _patrolling = true;
    }
    
    void SetRoute()
    {
        List<int> randomRoute = new List<int>(_allRouteIndexes);
        int routeCount = Random.Range(1, 6);
        _currentRoute.Clear();
        for (int i = 0; i < routeCount; i++)
        {
            int randomIndex = Random.Range(0, randomRoute.Count);
            _currentRoute.Add(randomRoute[randomIndex]);
            randomRoute.RemoveAt(randomIndex);
        }
    }

    Vector3 GetNextDestination(List<int> route)
    {
        var currentDestination = _patrolPoints[route[0]].transform.position;
        route.RemoveAt(0);
        return currentDestination;
    }
    
    #endregion
   
    // ------------------- CHASING
    #region Chasing
    IEnumerator GoToLastPlayerPosition()
    {
        Debug.Log("Started Chasing");
        _lockChasing = true;
        _navMeshAgent.ResetPath();
        if (_lastPlayerPosition != _nullPosition)
        {
            Debug.Log("Going to Last Player Pos");
            GoTo(_lastPlayerPosition);
        
            // Wait for path to be over
            yield return new WaitForSeconds(0.5f);
            while (_navMeshAgent.remainingDistance >= 0.1f)
            {
                yield return null;
            }
        }

        // Wait a few seconds at destination
        Debug.Log("Waiting at location");
        yield return new WaitForSeconds(5f);

        SearchForPlayer();
        //StartCoroutine(SearchForPlayer());
    }

    void SearchForPlayer()
    {
        var nearbyPointsRoute = GetNearbyPatrolPoints(_transform.position, shootingRange, 2);

        Debug.Log("Alert Route");
        StartCoroutine(TraverseRoute(nearbyPointsRoute));
        
        _lastPlayerPosition = _nullPosition;
        SetState(EnemyState.Patrolling);
    }
    
    #endregion
    
    // ------------------- UTILS
    #region Utils

    List<int> GetNearbyPatrolPoints(Vector3 center, float radius, int points)
    {
        Debug.Log("Started Get Nearby Points");
        Collider[] colliders = new Collider[points];
        
        int numberOfPoints = Physics.OverlapSphereNonAlloc(center, radius, colliders, _patrolPointsLayerMask);

        Debug.Log("Ran Overlap Sphere");
        Debug.Log("Number of Points: " + numberOfPoints);
        List<int> nearbyPoints = new List<int>();
        
        for (int i = 0; i < numberOfPoints; i++)
        {
            Debug.Log("Getting Index");
            Debug.Log("i = " + i);
            var col = colliders[i];
            
            var index = _patrolPoints.IndexOf(col);
            
            Debug.Log("Index: " + index);
            nearbyPoints.Add(index);
        }

        return nearbyPoints;
    }
    
    void SetState(EnemyState state)
    {
        this.state = state;
    }
    
    float CheckPlayerDistance()
    {
        return Vector3.Distance(player.transform.position, _transform.position);
    }

    void GetPlayerPosition()
    {
        if (_isInSeeRange)
        {
            _lastPlayerPosition = player.transform.position;
        }
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

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * seeRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * seeRange;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * seeRange);
        
        if (!_isInShootingRange)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Vector3 direction = (player.transform.position - transform.position).normalized * seeRange;
        direction.y *= 0;
        Gizmos.DrawRay(transform.position, direction);
    }
    
    #endregion
}
