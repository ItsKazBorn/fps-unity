using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoHolster
{
    private GunType gunType;
    private int maxAmmo;
    private int magSize;
    private int storedAmmo;
    private int currentAmmoInMag;
    private int reloadAmmount;

    public GunType GunType => gunType;
    public int StoredAmmo => storedAmmo;
    public int CurrentAmmoInMag => currentAmmoInMag;

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
