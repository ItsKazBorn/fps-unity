using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGranade : Weapon
{
    public float throwForce = 40f;
    public GameObject grenadePrefab;
    private Transform _transform;

    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

    public override void Fire()
    {
        GameObject grenade = Instantiate(grenadePrefab, _transform.position, _transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce);
    }
}
