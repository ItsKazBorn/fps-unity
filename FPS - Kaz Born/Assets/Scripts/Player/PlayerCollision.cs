using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private ThrowGranade granadeStorage;
    private WeaponSwitching weaponSwitching;

    private void Start()
    {
        weaponSwitching = FindObjectOfType<WeaponSwitching>().GetComponent<WeaponSwitching>();
        granadeStorage = FindObjectOfType<ThrowGranade>().GetComponent<ThrowGranade>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AmmoBox"))
        {
            weaponSwitching.GetSelectedWeapon().AddStoredAmmo();
            granadeStorage.AddStoredAmmo();
            Destroy(other.gameObject);
        }
    }

    
}
