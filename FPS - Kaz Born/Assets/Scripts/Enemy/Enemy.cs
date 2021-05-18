using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

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
    
    private NavMeshAgent _navMeshAgent;
    private Transform _transform;
    
    private Vector3 _initialPosition;
    private Vector3 _lastPlayerPosition;
    private Quaternion _initialRotation;

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
                GoTo(_lastPlayerPosition);
                StopAllCoroutines();
                StartCoroutine(GoToInitialPosition());
            }

            if (_isInShootingRange)
            {
                _rotateToInitialRotation = false;
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
            RotateToInitialRotation();
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
    
    IEnumerator GoToInitialPosition()
    {
        _rotateToInitialRotation = false;
        yield return new WaitForSeconds(0.5f);
        while (_navMeshAgent.remainingDistance >= 0.1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        GoTo(_initialPosition);
        yield return new WaitForSeconds(0.5f);
        while (_navMeshAgent.remainingDistance >= 0.1f)
        {
            yield return null;
        }

        _rotateToInitialRotation = true;
    }

    void RotateToInitialRotation()
    {
        if (_rotateToInitialRotation)
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, _initialRotation, 2);
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
        Collider[] overlaps = new Collider[10];
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

}
