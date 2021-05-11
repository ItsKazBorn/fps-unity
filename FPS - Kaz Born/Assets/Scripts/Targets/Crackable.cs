using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crackable : Shootable
{
    [SerializeField] private GameObject crackedVersion;
    
    protected override void Die()
    {
        Instantiate(crackedVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
