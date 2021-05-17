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
    [SerializeField] private float maxRadius = 5f;
    
    [Header("Gun")]
    [SerializeField] private EnemyGun _gun;
    

    
    
    private NavMeshAgent _navMeshAgent;
    private Transform _transform;
    
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    [SerializeField] private Transform _player;
    private Vector3 _lastPlayerPosition;

    private bool _rotateToInitialRotation;
    private bool _isInFOV;
    public bool IsInFOV
    {
        get { return _isInFOV; }
        set
        {
            if (value == _isInFOV)
                return;

            _isInFOV = value;
            if (!_isInFOV)
            {
                // execute code
                _rotateToInitialRotation = false;
                GoTo(_lastPlayerPosition);
                StopAllCoroutines();
                StartCoroutine(GoToInitialPosition());
            }

            if (_isInFOV)
            {
                _rotateToInitialRotation = false;
                StopAllCoroutines(); 
            }
        }
    }

    void Start()
    {
        _transform = transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _initialPosition = _transform.position;
        _initialRotation = _transform.rotation;
        _player = GameObject.Find("Player").transform;
        _lastPlayerPosition = _initialPosition;
    }

    void Update()
    {
        //CheckIfPlayerIsInShootingDistance();
        IsInFOV = InFOV();
        GetPlayerPosition();
        ShootPlayer();
        RotateToInitialRotation();
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
        if (_isInFOV)
        {
            _rotateToInitialRotation = false;
            Vector3 targetDirection = (_player.position - _transform.position).normalized;
            targetDirection.y *= 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, 25);
            
            // Fire Weapon
            _gun.Fire();
        }
    }
    
    void CheckIfPlayerIsInShootingDistance()
    {
        float distance = CheckPlayerDistance();

        if (distance <= maxRadius)
        {
            IsInFOV = true;
        }
        else
            IsInFOV = false;
    }
    
    float CheckPlayerDistance()
    {
        return Vector3.Distance(_player.transform.position, _transform.position);
    }

    void GetPlayerPosition()
    {
        if (_isInFOV)
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

    bool InFOV()
    {
        Collider[] overlaps = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(_transform.position, maxRadius, overlaps);

        for (int i = 0; i < count + 1; i++)
        {
            if (overlaps[i])
            {
                if (overlaps[i].transform == _player)
                {
                    Vector3 directionBetween = (_player.position - _transform.position).normalized;
                    directionBetween.y *= 0;

                    float angle = Vector3.Angle(_transform.forward, directionBetween);

                    if (angle <= maxAngle)
                    {
                        Vector3 direction = (_player.position - _transform.position);
                        RaycastHit hit;
                        if (Physics.Raycast(_transform.position, direction, out hit, maxRadius))
                        {
                            if (hit.transform == _player)
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
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!IsInFOV)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Vector3 direction = (_player.transform.position - transform.position).normalized * maxRadius;
        direction.y *= 0;
        Gizmos.DrawRay(transform.position, direction);
        
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

}
