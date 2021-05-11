using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Gun : Weapon
{
    [Header("Components")]
    [SerializeField] protected Camera fpsCam;
    
    [Header("Shoot")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] private float _gunRange = 100f;
    [SerializeField] private float _impactForce = 300f;
    [SerializeField] private float _fireRate = 15;
    [SerializeField] private float _nextTimeToFire = 0f;
    [SerializeField] protected float maxSpread = 0.1f;
    [SerializeField] protected float currentSpread = 0f;
    [SerializeField] protected bool isScoped = false;

    public bool IsScoped
    {
        get => isScoped;
        set => isScoped = value;
    }

    [Header("Visual Effects")]
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject bulletHole;

    protected override void OnEnable()
    {
        base.OnEnable();
        isScoped = false;
        animator.SetBool("Scoped", false);
    }

    protected virtual void OnDisable()
    {
        isScoped = false;
        animator.SetBool("Scoped", false);
    }

    public override void Fire()
    {
        base.Fire();
        _nextTimeToFire = Time.time + 1f / _fireRate;
        
        CalculateBullet();
    }

    public override void StopFiring()
    {
        base.StopFiring();
        currentSpread = 0f;
    }

    protected virtual void CalculateBullet()
    {
        Vector3 shootDirection = CalculateSpread();
        HitCalculation(shootDirection);
    }

    protected virtual Vector3 CalculateSpread()
    {
        // Get Shoot Direction
        Vector3 shootDirection = fpsCam.transform.forward;
        // Get Random Direction
        Vector3 spread = new Vector3(UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)).normalized;
        // Multiply by intensity of spread
        float multiplier = UnityEngine.Random.Range(0f, currentSpread);
        spread *= multiplier;
        // Change shoot direction by spread
        shootDirection = shootDirection + fpsCam.transform.TransformDirection(
            new Vector3(spread.x,spread.y));
        
        // Make spread larger for next firing
        currentSpread += 0.01f;
        currentSpread = Mathf.Clamp(currentSpread, 0f, maxSpread);

        return shootDirection;
    }

    protected void HitCalculation(Vector3 shootDirection)
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, shootDirection, out hit, _gunRange))
        {
            DoDamage(hit);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * _impactForce);
            
            // Spawn impact effect
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 1f);
            Instantiate(bulletHole, hit.point, Quaternion.LookRotation((hit.normal)));
        }
    }

    protected virtual void DoDamage(RaycastHit hit)
    {
        Shootable target = hit.collider.gameObject.GetComponent<Shootable>();
        if (target != null)
            target.TakeDamage(damage);
    }

    public override bool CanFire()
    {
        if (Time.time >= _nextTimeToFire && !isReloading)
            return true;
        return false;
    }

    public override bool CanReload()
    {
        if (!isFiring && !isReloading && !isScoped)
            return true;
        return false;
    }

    public virtual void Scope()
    {
        isScoped = true;
        animator.SetBool("Scoped", true);
    }

    public virtual void UnScope()
    {
        isScoped = false;
        animator.SetBool("Scoped", false);
    }
}
