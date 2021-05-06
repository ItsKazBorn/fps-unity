using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public WeaponHolster weaponHolster;
    

    private bool canShoot = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            weaponHolster.ChangeSelectedWeapon(Input.GetAxis("Mouse ScrollWheel"));
        
        if (Input.GetKeyDown(KeyCode.R))
            weaponHolster.ReloadSelectedWeapon();

        if (Input.GetButton("Fire1") && canShoot)
            weaponHolster.FireSelectedWeapon();
        else
            weaponHolster.StopFiringSelectedWeapon();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter Trigger");
        Debug.Log("Other Tag: " + other.gameObject.tag);
        if (other.gameObject.CompareTag("AmmoBox"))
        {
            Debug.Log("Is AmmoBox");
            AmmoBox ammoBox = other.gameObject.GetComponent<AmmoBox>();
            weaponHolster.AddStoredAmmoTo(ammoBox.gunType);
            ammoBox.Deactivate();
        }
    }
}
