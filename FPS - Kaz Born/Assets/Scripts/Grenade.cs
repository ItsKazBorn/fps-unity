using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public float radius = 5f;
    public float force = 700f;
    public GameObject explosionEffect;
    public GameObject granadeExplosionPrefab;
    
    
    private float countdown;
    private bool hasExploded = false;
    
    

    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0 && !hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;
        
        
        // Particle effect
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosion, 2f);
        GameObject explosionSound = Instantiate(granadeExplosionPrefab, transform.position, transform.rotation);
        Destroy(explosionSound, 2f);

        // Get Objects in blast radius
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in collidersToDestroy)
        {
            // Damage
            Shootable shootable = nearbyObject.GetComponent<Shootable>();
            if (shootable != null)
            {
                shootable.TakeDamage(force);
            }

            Grenade grenade = nearbyObject.GetComponent<Grenade>();
            if (grenade != null && !grenade.hasExploded)
            {
                grenade.Explode();
            }
        }
        
        // Get Objects in blast radius
        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in collidersToMove)
        {
            // Add force
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        // Remove Grenade
        Destroy(gameObject);
    }
    
    
}
