using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : Gun
{
    
    [Header("Enemy Gun Class")]
    [SerializeField] private int burstAmmount = 3;
    [SerializeField] private int minBurst = 2;
    [SerializeField] private int maxBurst = 6;
    private int currentBurst = 0;
    [SerializeField] private float reloadBurstTime = 1f;

    public override void Fire()
    {
        if (CanFire())
        {
            currentBurst++;
            base.Fire();
        }
        else if (!isReloading && currentBurst >= burstAmmount)
            StartCoroutine(ReloadBurst());
    }

    public void StopFiring()
    {
        animator.SetBool("Firing", false);
    }
    
    IEnumerator ReloadBurst()
    {
        StopFiring();
        isReloading = true;
        yield return new WaitForSeconds(reloadBurstTime);
        currentBurst = 0;
        burstAmmount = Random.Range(minBurst, maxBurst);
        IsReloading = false;
    }
    
    public override bool CanFire()
    {
        if (Time.time >= _nextTimeToFire && !isReloading && currentBurst < burstAmmount)
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

    protected override void DoDamage(RaycastHit hit)
    {
        GameObject hitObj = hit.collider.gameObject;
        if (hitObj.CompareTag("Player"))
        {
            PlayerHealth playerHealth = hitObj.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damage);
        }
        else
        {
            Shootable target = hitObj.GetComponent<Shootable>();
            if (target)
                target.TakeDamage(damage);
        }
    }
}
