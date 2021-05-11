using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Sprint();
        Jump();
        Move();
        
        SwitchWeapons();
        ReloadWeapon();
        FireWeapon();
        ScopeGun();

        QInput();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AmmoBox"))
        {
            // Put in Events
            AmmoBox ammoBox = other.gameObject.GetComponent<AmmoBox>();
            InputEvents.current.AmmoPickUp(ammoBox.GunType);
        }
    }

    void Move()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            InputEvents.current.WASDPressed(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
            InputEvents.current.JumpPressed();
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
            InputEvents.current.SprintPressed();
        
        if (Input.GetButtonUp("Sprint"))
            InputEvents.current.SprintReleased();
    }
    
    
    void SwitchWeapons()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            InputEvents.current.ScrollWheel(Input.GetAxis("Mouse ScrollWheel"));
        
        if (Input.GetButtonDown("SwitchWeapon1"))
            InputEvents.current.WeaponSelectPress(0);
        
        if (Input.GetButtonDown("SwitchWeapon2"))
            InputEvents.current.WeaponSelectPress(1);
        
        if (Input.GetButtonDown("SwitchWeapon3"))
            InputEvents.current.WeaponSelectPress(2);
        
        if (Input.GetButtonDown("SwitchWeapon4"))
            InputEvents.current.WeaponSelectPress(3);
        
        if (Input.GetButtonDown("SwitchWeapon5"))
            InputEvents.current.WeaponSelectPress(4);
        
        if (Input.GetButtonDown("SwitchWeapon6"))
            InputEvents.current.WeaponSelectPress(5);
    }

    void ReloadWeapon()
    {
        if (Input.GetButtonDown("Reload"))
            InputEvents.current.ReloadPressed();
    }

    void FireWeapon()
    {
        if (Input.GetButton("Fire1"))
            InputEvents.current.FirePressed();
        
        else
            InputEvents.current.FireReleased();
    }
    
    void ScopeGun()
    {
        if (Input.GetButtonDown("Fire2"))
            InputEvents.current.AimPress();

        if (Input.GetButtonUp("Fire2"))
            InputEvents.current.AimRelease();
    }
    
    // TESTING EVENT SYSTEM
    private void QInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnQPress();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            OnQRelease();
        }
    }

    private void OnQPress()
    {
        GameEvents.current.QPressed();
    }

    private void OnQRelease()
    {
        GameEvents.current.QReleased();
    }
    // ------------------ END TESTING
}
