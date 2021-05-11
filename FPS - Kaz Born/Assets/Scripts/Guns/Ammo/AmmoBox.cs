using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [SerializeField] private GunType gunType;
    public GunType GunType => gunType;

    [SerializeField] private float respawnTime = 5f;

    private BoxCollider _boxCollider;
    private List<Transform> _children = new List<Transform>();
    private Transform _transform;
    

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _boxCollider = GetComponent<BoxCollider>();
        foreach (Transform child in _transform)
        {
            _children.Add(child);
        }

        InputEvents.current.onAmmoPickUp += Deactivate;
    }

    private void OnDestroy()
    {
        InputEvents.current.onAmmoPickUp -= Deactivate;
    }

    private void Update()
    {
        _transform.Rotate(0, 45 * Time.deltaTime, 0);
    }

    public void Deactivate(GunType gunType)
    {
        if (gunType == this.gunType)
        {
            _boxCollider.enabled = false;
            foreach (Transform child in _children)
            {
                child.gameObject.SetActive(false);
            }
            StartCoroutine(Activate());
        }
    }
    
    public void Deactivate()
    {
        _boxCollider.enabled = false;
        foreach (Transform child in _children)
        {
            child.gameObject.SetActive(false);
        }
        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(respawnTime);
        _boxCollider.enabled = true;
        foreach (Transform child in _children)
        {
            child.gameObject.SetActive(true);
        }
    }
}
