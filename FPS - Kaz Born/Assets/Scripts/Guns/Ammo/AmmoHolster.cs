using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoHolster
{
    public GunType gunType;

    public int maxAmmo;
    public int magSize;
    public int storedAmmo;
    public int currentAmmoInMag;
    public int reloadAmmount;

    public void Initialize(GunType newGunType, int newMaxAmmo, int newMagSize)
    {
        gunType = newGunType;
        maxAmmo = newMaxAmmo;
        magSize = newMagSize;
        storedAmmo = newMagSize;
        currentAmmoInMag = newMagSize;
        reloadAmmount = newMagSize;
    }

    public void SetReloadAmmount(int newReloadAmmount)
    {
        reloadAmmount = newReloadAmmount;
    }

    public int CalculateAmmountToReload()
    {
        int ammountToReload = reloadAmmount;
        if (reloadAmmount == magSize)
            ammountToReload -= currentAmmoInMag;

        if (storedAmmo >= ammountToReload)
            return ammountToReload;
        return storedAmmo;
    }

    public void ReloadMag(int ammountToReload)
    {
        currentAmmoInMag += ammountToReload;
        storedAmmo -= ammountToReload;
    }

    public void AddStoredAmmo()
    {
        int ammoAmmount = magSize;
        int spaceLeft =  maxAmmo - storedAmmo;

        if (spaceLeft > 0)
        {
            if (ammoAmmount < spaceLeft)
                storedAmmo += ammoAmmount;
            else
                storedAmmo += spaceLeft;
        }
    }

    public void UseAmmo()
    {
        currentAmmoInMag--;
    }
}
