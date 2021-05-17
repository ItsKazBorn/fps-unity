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
    
    IEnumerator ReloadBurst()
    {
        animator.SetBool("Firing", false);
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
}
