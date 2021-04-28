using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Gun : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Camera fpsCam;
    public AudioSource audioSource;
    
    [Header("Shoot")]
    public float damage = 10f;
    private float gunRange = 100f;
    private float impactForce = 300f;
    public float fireRate = 15;
    private float nextTimeToFire = 0f;
    public float maxSpread = 0.1f;
    private float currentSpread = 0f;
    
    [Header("Reload")]
    public int magSize = 30;
    private int storedAmmo;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;
    public TextMeshProUGUI ammoText;

    [Header("Visual Effects")]
    public GameObject muzzleFalsh;
    public Transform muzzleLocation;
    public GameObject impactEffect;
    public GameObject bulletHole;

    [Header("Sound Effects")] 
    public AudioClip gunshotSound;
    public AudioClip reloadSound;
    public AudioClip takeMagSound;
    
    private void Start()
    {
        currentAmmo = magSize;
        storedAmmo = magSize;
        ammoText.text = currentAmmo + " / " + storedAmmo;
    }

    private void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReloading && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
        
        if (!isReloading && currentAmmo > 0 && Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        // Stop spread
        if (!Input.GetButton("Fire1"))
        {
            currentSpread = 0f;
            animator.SetBool("Firing", false);
        }

        
    }

    void Shoot()
    {
        animator.SetBool("Firing", true);
        currentAmmo--; // Use 1 ammo
        audioSource.PlayOneShot(gunshotSound); // Play Gunshot Sound
        ammoText.text = currentAmmo + " / " + storedAmmo; //  Update ammo text
        
        // Spawn muzzleflash
        GameObject muzzleFalshGO = Instantiate(muzzleFalsh, muzzleLocation);
        Destroy(muzzleFalshGO, 1f);
        
        
        // Check if hit a target
        RaycastHit hit;

        // Spread
        Vector3 shootDirection = fpsCam.transform.forward;
        /*
        shootDirection = shootDirection + fpsCam.transform.TransformDirection(
            new Vector3(UnityEngine.Random.Range(-currentSpread, currentSpread),
                UnityEngine.Random.Range(-currentSpread, currentSpread)));
        */
        Vector3 spread = new Vector3(UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)).normalized;
        float multiplier = UnityEngine.Random.Range(0f, currentSpread);
        spread *= multiplier;
        shootDirection = shootDirection + fpsCam.transform.TransformDirection(
            new Vector3(spread.x,spread.y));
        
        currentSpread += 0.01f;
        currentSpread = Mathf.Clamp(currentSpread, 0f, maxSpread);

        // Hit Calculations
        if (Physics.Raycast(fpsCam.transform.position, shootDirection, out hit, gunRange))
        {
            Shootable target = hit.transform.GetComponent<Shootable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            // Spawn impact effect
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 1f);
            Instantiate(bulletHole, hit.point, Quaternion.LookRotation((hit.normal)));
        }
        
        
    }

    IEnumerator Reload()
    {
        int ammountToReload = magSize - currentAmmo;
        int reloadAmmount = 0;
        if (storedAmmo >= ammountToReload)
        {
            reloadAmmount = ammountToReload;
        }
        else
        {
            reloadAmmount = storedAmmo - currentAmmo;
        }

        if (reloadAmmount > 0)
        {
            audioSource.PlayOneShot(reloadSound);
            isReloading = true;
            animator.SetBool("Reloading", true);

            yield return new WaitForSeconds(reloadTime - .25f);
            currentAmmo += reloadAmmount;
            storedAmmo -= reloadAmmount;
            ammoText.text = currentAmmo + " / " + storedAmmo;
            animator.SetBool("Reloading", false);
        
            yield return new WaitForSeconds(.25f);
            isReloading = false;
        }
    }

    public void AddStoredAmmo()
    {
        audioSource.PlayOneShot(takeMagSound);
        storedAmmo += magSize;
        ammoText.text = currentAmmo + " / " + storedAmmo;
    }
}
