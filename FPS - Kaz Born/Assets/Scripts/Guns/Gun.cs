﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Gun : Weapon
{
    [Header("Components")]
    [SerializeField] protected Transform originPoint;
    
    [Header("Shoot")]
    [SerializeField] protected float damage = 10f;
    private float _gunRange = 100f;
    private float _impactForce = 300f;
    [SerializeField] protected float _fireRate = 15;
    protected float _nextTimeToFire = 0f;
    [SerializeField] protected float maxSpread = 0.1f;
    protected float currentSpread = 0f;
    protected bool isScoped = false;

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
        GameEvents.current.SpreadChanged(currentSpread);
    }

    protected virtual void CalculateBullet()
    {
        Vector3 shootDirection = CalculateSpread();
        HitCalculation(shootDirection);
    }

    protected virtual Vector3 CalculateSpread()
    {
        // Get Shoot Direction
        Vector3 shootDirection = originPoint.forward;
        // Get Random Direction
        Vector3 spread = new Vector3(UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)).normalized;
        // Multiply by intensity of spread
        float multiplier = UnityEngine.Random.Range(0f, currentSpread);
        spread *= multiplier;
        // Change shoot direction by spread
        shootDirection = shootDirection + originPoint.TransformDirection(
            new Vector3(spread.x,spread.y));
        
        // Make spread larger for next firing
        currentSpread += 0.01f;
        currentSpread = Mathf.Clamp(currentSpread, 0f, maxSpread);
        GameEvents.current.SpreadChanged(currentSpread);

        return shootDirection;
    }

    protected void HitCalculation(Vector3 shootDirection)
    {
        RaycastHit hit;
        if (Physics.Raycast(originPoint.transform.position, shootDirection, out hit, _gunRange))
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
        if (hit.collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
        }
        else
        {
            Shootable target = hit.collider.gameObject.GetComponent<Shootable>();
            if (target)
                target.TakeDamage(damage);
        }
    }

    public override bool CanFire()
    {
        if (Time.time >= _nextTimeToFire && !isReloading)
        {
            if (!autoFire)
            {
                if (!isFiring)
                    return true;
                return false;
            }
            return true;
        }
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
