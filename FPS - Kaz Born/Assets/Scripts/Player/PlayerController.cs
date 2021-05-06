using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public WeaponHolster weaponHolster;
    private PlayerMovement playerMovement;

    private bool canShoot = true;
    
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Move();

        SwitchWeapons();
        ReloadWeapon();
        FireWeapon();
        ScopeGun();
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

    void Move()
    {
        playerMovement.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
            playerMovement.Jump();
    }

    void Sprint()
    {
        playerMovement.Sprint(Input.GetKey(KeyCode.LeftShift));
    }
    

    void SwitchWeapons()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            weaponHolster.ChangeSelectedWeapon(Input.GetAxis("Mouse ScrollWheel"));
    }

    void ReloadWeapon()
    {
        if (Input.GetKeyDown(KeyCode.R))
            weaponHolster.ReloadSelectedWeapon();
    }

    void FireWeapon()
    {
        if (Input.GetButton("Fire1") && canShoot)
            weaponHolster.FireSelectedWeapon();
        else
            weaponHolster.StopFiringSelectedWeapon();
    }

    void ScopeGun()
    {
        
    }
}
