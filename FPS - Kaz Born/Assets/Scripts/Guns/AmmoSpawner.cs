using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    public GameObject ammoBoxPrefab;
    public GameObject[] spawnLocations;
    public float timeToSpawn = 2f;

    private bool spawning = false;

    void Update()
    {
        int ammoCount = FindObjectsOfType<AmmoBox>().Length;

        if (ammoCount < 1 && !spawning)
            StartCoroutine(SpawnAmmoBox());
    }

    IEnumerator SpawnAmmoBox()
    {
        spawning = true;
        yield return new WaitForSeconds(timeToSpawn);

        int randomIndex = Random.Range(0, spawnLocations.Length);

        Instantiate(ammoBoxPrefab, spawnLocations[randomIndex].transform.position, ammoBoxPrefab.transform.rotation);

        spawning = false;
    }
}
