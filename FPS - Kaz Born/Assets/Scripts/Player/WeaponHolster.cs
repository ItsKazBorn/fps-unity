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
    private List<Weapon> _weapons = new List<Weapon>();

    private List<AmmoHolster> _ammoHolsters = new List<AmmoHolster>();
    
    private int selectedWeapon = 0;
    
    void Start()
    {
        foreach (Transform obj in transform)
        {
            Weapon weapon = obj.GetComponent<Weapon>();
            if (weapon != null)
            {
                _weapons.Add(weapon);
                weapon.gameObject.SetActive(false);
            }
        }
        
        InitAmmo();
        SelectWeapon(1);
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

    private static readonly Dictionary<GunType, (int, int)> registry =  new Dictionary<GunType, (int, int)>()
    {
        {GunType.Rifle, (90, 30)},
        {GunType.Handgun, (30, 10)},
        {GunType.Shotgun, (40, 5)},
        {GunType.Sniper, (30, 5)},
        {GunType.SubMachine, (120, 30)},
        {GunType.Grenade, (3, 1)}
    };
    
    void InitAmmo()
    {
        for (int i = 0; i < _weapons.Count; i++)
        {
            Weapon gun = _weapons[i];
            AmmoHolster ammoHolster = new AmmoHolster();

            var tuple = registry[gun.gunType];
            ammoHolster.Initialize(gun.gunType, tuple.Item1, tuple.Item2);
            
            if (gun.gunType == GunType.Shotgun)
                ammoHolster.SetReloadAmmount(1);
            
            _ammoHolsters.Add(ammoHolster);
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
            if (selectedWeapon >= _weapons.Count - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        else if (axis < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = _weapons.Count - 1;
            else
                selectedWeapon--;
        }

        if (previousWeapon != selectedWeapon)
            SelectWeapon(previousWeapon);
    }
    
    void SelectWeapon(int previousWeapon)
    {
        _weapons[previousWeapon].gameObject.SetActive(false);
        _weapons[selectedWeapon].gameObject.SetActive(true);
        UpdateAmmoText();
    }

    public Weapon GetSelectedWeapon()
    {
        return _weapons[selectedWeapon];
    }
    
    AmmoHolster GetSelectedAmmoHolster()
    {
        foreach (AmmoHolster ammo in _ammoHolsters)
        {
            if (ammo.gunType == GetSelectedWeapon().gunType)
                return ammo;
        }
        return null;
    }
    
    Weapon GetWeaponOfType(GunType gunType)
    {
        foreach (Weapon weapon in _weapons)
        {
            if (weapon.gunType == gunType)
                return weapon;
        }
        return null;
    }

    AmmoHolster GetAmmoHolsterOfType(GunType gunType)
    {
        foreach (AmmoHolster ammoHolster in _ammoHolsters)
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
