using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGranade : Weapon
{
    public float throwForce = 40f;
    public GameObject grenadePrefab;

    public override void Fire()
    {
        GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce);
    }
}
