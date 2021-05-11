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
    protected float reloadTime = 1f;

    [Header("Animation")]
    [SerializeField] protected Animator animator;

    [Header("Audio")] 
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _reloadSound;
    [SerializeField] private AudioClip _takeAmmoSound;
    [SerializeField] private AudioClip _fireSound;
    
    protected bool isFiring = false;

    public GunType GunType => gunType;
    public Animator Animator => animator;

    protected virtual void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    public virtual bool CanFire()
    {
        if (!isReloading)
            return true;
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
}
