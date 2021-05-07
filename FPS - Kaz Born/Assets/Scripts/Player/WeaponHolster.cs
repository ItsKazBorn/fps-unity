using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GunType
{
    Rifle,
    Handgun,
    Sniper,
    Shotgun,
    SubMachine,
    Grenade
}

public class WeaponHolster : MonoBehaviour
{
    private List<Weapon> weapons = new List<Weapon>();

    private List<AmmoHolster> ammoHolsters = new List<AmmoHolster>();
    
    public int selectedWeapon = 0;
    public AmmoText ammoText;
    
    void Start()
    {
        foreach (Transform obj in transform)
        {
            Weapon weapon = obj.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapons.Add(weapon);
            }
        }
        
        InitAmmo();
        SelectWeapon(selectedWeapon);
    }

    public void FireSelectedWeapon()
    {
        AmmoHolster holster = GetSelectedAmmoHolster();

        Weapon weapon = GetSelectedWeapon();
        
        if (holster.currentAmmoInMag > 0 && weapon.CanFire())
        {
            // Can Shoot
            weapon.Fire();
            holster.UseAmmo();
            UpdateAmmoText();
        }
    }

    public void StopFiringSelectedWeapon()
    {
        GetSelectedWeapon().StopFiring();
    }

    public void ReloadSelectedWeapon()
    {
        if (GetSelectedWeapon().CanReload())
        {
            AmmoHolster holster = GetSelectedAmmoHolster();

            int reloadAmmount = holster.CalculateAmmountToReload();

            if (reloadAmmount > 0)
            {
                holster.ReloadMag(reloadAmmount);
                GetSelectedWeapon().Reload();
                UpdateAmmoText();
            }
        }
    }
    
    void InitAmmo()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            Weapon gun = weapons[i];
            AmmoHolster ammoHolster = new AmmoHolster();
            switch (gun.gunType)
            {
                case GunType.Rifle:
                    ammoHolster.Initialize(GunType.Rifle, 90, 30);
                    break;
                case GunType.Handgun:
                    ammoHolster.Initialize(GunType.Handgun, 30, 10);
                    break;
                case GunType.Shotgun:
                    ammoHolster.Initialize(GunType.Shotgun, 40, 5);
                    ammoHolster.SetReloadAmmount(1);
                    break;
                case GunType.Sniper:
                    ammoHolster.Initialize(GunType.Sniper, 30, 5);
                    break;
                case GunType.SubMachine:
                    ammoHolster.Initialize(GunType.SubMachine, 120, 30);
                    break;
                case GunType.Grenade:
                    ammoHolster.Initialize(GunType.Grenade, 3, 1);
                    break;
            }
            ammoHolsters.Add(ammoHolster);
        }
    }

    public void AddStoredAmmoTo(GunType gunType)
    {
        GetAmmoHolsterOfType(gunType).AddStoredAmmo();
        UpdateAmmoText();
    }
    
    public void ChangeSelectedWeapon(float axis)
    {
        int previousWeapon = selectedWeapon;
        if (axis > 0f)
        {
            if (selectedWeapon >= weapons.Count - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        else if (axis < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = weapons.Count - 1;
            else
                selectedWeapon--;
        }

        if (previousWeapon != selectedWeapon)
            SelectWeapon(previousWeapon);
    }
    
    void SelectWeapon(int previousWeapon)
    {
        weapons[previousWeapon].gameObject.SetActive(false);
        weapons[selectedWeapon].gameObject.SetActive(true);
        UpdateAmmoText();
    }

    public Weapon GetSelectedWeapon()
    {
        return weapons[selectedWeapon];
    }
    
    AmmoHolster GetSelectedAmmoHolster()
    {
        foreach (AmmoHolster ammo in ammoHolsters)
        {
            if (ammo.gunType == GetSelectedWeapon().gunType)
                return ammo;
        }
        return null;
    }
    
    Weapon GetWeaponOfType(GunType gunType)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.gunType == gunType)
                return weapon;
        }
        return null;
    }

    AmmoHolster GetAmmoHolsterOfType(GunType gunType)
    {
        foreach (AmmoHolster ammoHolster in ammoHolsters)
        {
            if (ammoHolster.gunType == gunType)
            {
                return ammoHolster;
            }
        }
        return null;
    }

    void UpdateAmmoText()
    {
        var holster = GetSelectedAmmoHolster();
        GameEvents.current.AmmoChanged(holster.currentAmmoInMag, holster.storedAmmo);
    }
}
