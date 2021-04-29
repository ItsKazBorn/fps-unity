using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThrowGranade : MonoBehaviour
{
    public float throwForce = 40f;
    public GameObject grenadePrefab;

    [Header("Ammo")] 
    public int ammo = 10;
    public int reloadAmmount = 1;
    public TextMeshProUGUI ammoText;

    private void Start()
    {
        ammoText.text = ammo + " ";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && ammo > 0)
            ThrowGrenade();
    }

    void ThrowGrenade()
    {
        ammo--;
        ammoText.text = ammo + " ";
        GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce);
    }

    public void AddAmmo()
    {
        ammo += reloadAmmount;
        ammoText.text = ammo + " ";
    }
}
