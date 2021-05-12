using System;
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
    
    private int selectedWeapon = 0;
    
    private static readonly Dictionary<GunType, (int, int)> registry =  new Dictionary<GunType, (int, int)>()
    {
        {GunType.Rifle, (90, 30)},
        {GunType.Handgun, (30, 10)},
        {GunType.Shotgun, (40, 5)},
        {GunType.Sniper, (30, 5)},
        {GunType.SubMachine, (120, 30)},
        {GunType.Grenade, (3, 1)}
    };
    
    void Start()
    {
        foreach (Transform obj in transform)
        {
            Weapon weapon = obj.GetComponent<Weapon>();
            if (weapon)
            {
                weapons.Add(weapon);
                weapon.gameObject.SetActive(false);
            }
        }
        InitAmmo();
        SelectWeapon(selectedWeapon);

        InputEvents.current.onFirePressed += FireSelectedWeapon;
        InputEvents.current.onFireReleased += StopFiringSelectedWeapon;
        InputEvents.current.onReloadPressed += ReloadSelectedWeapon;
        InputEvents.current.onWeaponSelectPress += ChangeSelectedWeaponByIndex;
        InputEvents.current.onScrollWheel += ChangeSelectedWeaponByAxis;
        InputEvents.current.onAimPress += ScopeSelectedGun;
        InputEvents.current.onAimRelease += UnScopeSelectedGun;
        InputEvents.current.onAmmoPickUp += AddStoredAmmoTo;
    }

    private void OnDestroy()
    {
        InputEvents.current.onFirePressed -= FireSelectedWeapon;
        InputEvents.current.onFireReleased -= StopFiringSelectedWeapon;
        InputEvents.current.onReloadPressed -= ReloadSelectedWeapon;
        InputEvents.current.onWeaponSelectPress -= ChangeSelectedWeaponByIndex;
        InputEvents.current.onScrollWheel -= ChangeSelectedWeaponByAxis;
        InputEvents.current.onAimPress -= ScopeSelectedGun;
        InputEvents.current.onAimRelease -= UnScopeSelectedGun;
        InputEvents.current.onAmmoPickUp -= AddStoredAmmoTo;
    }

    public void FireSelectedWeapon()
    {
        AmmoHolster holster = GetSelectedAmmoHolster();
        Weapon weapon = GetSelectedWeapon();
        
        if (holster.CurrentAmmoInMag > 0 && weapon.CanFire())
        {
            
            // Can Shoot
            weapon.Fire();
            holster.UseAmmo();
            UpdateAmmoText();
            GameEvents.current.WeaponFire();
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

    public void ScopeSelectedGun()
    {
        Gun gun = GetSelectedWeapon() as Gun;
        if (gun)
            gun.Scope();
    }

    public void UnScopeSelectedGun()
    {
        Gun gun = GetSelectedWeapon() as Gun;
        if (gun)
            gun.UnScope();
    }

    void InitAmmo()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            Weapon gun = weapons[i];
            AmmoHolster ammoHolster = new AmmoHolster();
            
            var tuple = registry[gun.GunType];
            ammoHolster.Initialize(gun.GunType, tuple.Item1, tuple.Item2);
            
            if (gun.GunType == GunType.Shotgun)
                ammoHolster.SetReloadAmmount(1);
            
            ammoHolsters.Add(ammoHolster);
        }
    }

    public void AddStoredAmmoTo(GunType gunType)
    {
        GetAmmoHolsterOfType(gunType).AddStoredAmmo();
        GetSelectedWeapon().AddStoredAmmo();
        UpdateAmmoText();
    }
    
    public void ChangeSelectedWeaponByAxis(float axis)
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

    public void ChangeSelectedWeaponByIndex(int index)
    {
        int previousWeapon = selectedWeapon;
        selectedWeapon = index;
        SelectWeapon(previousWeapon);
    }
    
    void SelectWeapon(int previousWeapon)
    {
        StartCoroutine(ChangeWeapon(previousWeapon));
        //weapons[previousWeapon].gameObject.SetActive(false);
        //weapons[selectedWeapon].gameObject.SetActive(true);
        //UpdateAmmoText();
    }

    IEnumerator ChangeWeapon(int previousWeapon)
    {
        Weapon previous = weapons[previousWeapon];
        previous.Animator.SetTrigger("Deselect");
        yield return new WaitForSeconds(.2f);
        
        previous.gameObject.SetActive(false);

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
            if (ammo.GunType == GetSelectedWeapon().GunType)
                return ammo;
        }
        return null;
    }
    
    Weapon GetWeaponOfType(GunType gunType)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.GunType == gunType)
                return weapon;
        }
        return null;
    }

    AmmoHolster GetAmmoHolsterOfType(GunType gunType)
    {
        foreach (AmmoHolster ammoHolster in ammoHolsters)
        {
            if (ammoHolster.GunType == gunType)
            {
                return ammoHolster;
            }
        }
        return null;
    }

    void UpdateAmmoText()
    {
        var holster = GetSelectedAmmoHolster();
        GameEvents.current.AmmoChanged(holster.CurrentAmmoInMag, holster.StoredAmmo);
    }
}
