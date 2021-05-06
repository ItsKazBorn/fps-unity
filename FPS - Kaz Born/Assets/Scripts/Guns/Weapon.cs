using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    public GunType gunType;
    
    protected bool isFiring = false;
    protected bool isReloading = false;
    protected float reloadTime = 1f;

    [Header("Animation")]
    public Animator animator;

    [Header("Audio")] 
    public AudioSource audioSource;
    public AudioClip reloadSound;
    public AudioClip takeAmmoSound;
    public AudioClip fireSound;

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
        audioSource.PlayOneShot(fireSound);
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
        audioSource.PlayOneShot(reloadSound);
        isReloading = true;
        animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime - .25f);
        animator.SetBool("Reloading", false);
        
        yield return new WaitForSeconds(.25f);
        isReloading = false;
    }

    public virtual void AddStoredAmmo()
    {
        audioSource.PlayOneShot(takeAmmoSound);
    }
}
