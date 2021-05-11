using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeExplosion : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(explosionSound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
