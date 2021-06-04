using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GunType gunType;

    protected bool isReloading = false;

    public bool IsReloading
    {
        get => isReloading;
        set => isReloading = value;
    }

    protected float reloadTime = 1f;
    [SerializeField] protected bool autoFire = true;
    [SerializeField] private float recoilAmmount = 5f;

    [Header("Animation")]
    [SerializeField] protected Animator animator;

    [Header("Audio")] 
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _reloadSound;
    [SerializeField] private AudioClip _takeAmmoSound;
    [SerializeField] private AudioClip _fireSound;
    [SerializeField] protected float audioIntensity;
    [SerializeField] protected LayerMask listenerMask;
    protected Collider[] _listeners;
    
    
    protected bool isFiring = false;

    public GunType GunType => gunType;
    public Animator Animator => animator;
    public float RecoilAmmount => recoilAmmount;
    
    protected virtual void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    public virtual bool CanFire()
    {
        if (!isReloading)
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

    public virtual bool CanReload()
    {
        if (!isFiring && !isReloading)
            return true;
        return false;
    }

    public virtual void Fire()
    {
        animator.SetBool("Firing", true);
        isFiring = true;
        _audioSource.PlayOneShot(_fireSound);
        if (!autoFire)
            StartCoroutine(StopFiringAnimation());
    }

    IEnumerator StopFiringAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Firing", false);
    }

    public virtual void StopFiring()
    {
        animator.SetBool("Firing", false);
        isFiring = false;
    }

    public  void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    protected IEnumerator ReloadCoroutine()
    {
        _audioSource.PlayOneShot(_reloadSound);
        isReloading = true;
        animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime - .25f);
        animator.SetBool("Reloading", false);
        
        yield return new WaitForSeconds(.25f);
        isReloading = false;
    }

    public void AddStoredAmmo()
    {
        _audioSource.PlayOneShot(_takeAmmoSound);
    }

    protected void EmitSound(float intensity, Vector3 sourcePosition)
    {
        float maxDistance = Mathf.Sqrt(audioIntensity / 0.1f);
        if (Physics.OverlapSphereNonAlloc(sourcePosition, maxDistance, _listeners, listenerMask) > 0)
        {
            foreach (var listener in _listeners)
            {
                // Tell listener about sound
            }
        }
    }
}
