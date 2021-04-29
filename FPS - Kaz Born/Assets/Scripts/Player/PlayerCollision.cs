using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private WeaponSwitching weaponSwitching;

    private void Start()
    {
        weaponSwitching = FindObjectOfType<WeaponSwitching>().GetComponent<WeaponSwitching>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AmmoBox"))
        {
            weaponSwitching.GetSelectedWeapon().AddStoredAmmo();
            Destroy(other.gameObject);
        }
    }

    
}
