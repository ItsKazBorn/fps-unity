using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private Gun gun;

    private void Start()
    {
        gun = FindObjectOfType<Gun>().GetComponent<Gun>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AmmoBox"))
        {
            gun.AddStoredAmmo();
            Destroy(other.gameObject);
        }
    }

    
}
