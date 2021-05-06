using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Gun : Weapon
{
    [Header("Components")]
    public Camera fpsCam;
    
    [Header("Shoot")]
    public float damage = 10f;
    private float gunRange = 100f;
    private float impactForce = 300f;
    public float fireRate = 15;
    public float nextTimeToFire = 0f;
    public float maxSpread = 0.1f;
    protected float currentSpread = 0f;
    public bool isScoped = false;

    [Header("Visual Effects")]
    public GameObject impactEffect;
    public GameObject bulletHole;

    protected override void OnEnable()
    {
        base.OnEnable();
        isScoped = false;
    }

    void Update()
    {
        Scope();
    }

    public override void Fire()
    {
        base.Fire();
        nextTimeToFire = Time.time + 1f / fireRate;
        
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
        if (Physics.Raycast(fpsCam.transform.position, shootDirection, out hit, gunRange))
        {
            DoDamage(hit);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            
            // Spawn impact effect
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 1f);
            Instantiate(bulletHole, hit.point, Quaternion.LookRotation((hit.normal)));
        }
    }

    protected virtual void DoDamage(RaycastHit hit)
    {
        Shootable target = hit.transform.GetComponent<Shootable>();
        if (target != null)
            target.TakeDamage(damage);
    }

    protected virtual void Scope()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            isScoped = true;
            animator.SetBool("Scoped", isScoped);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isScoped = false;
            animator.SetBool("Scoped", isScoped);
        }
    }

    public override bool CanFire()
    {
        if (Time.time >= nextTimeToFire && !isReloading)
            return true;
        return false;
    }

    public override bool CanReload()
    {
        if (!isFiring && !isReloading && !isScoped)
            return true;
        return false;
    }
}
