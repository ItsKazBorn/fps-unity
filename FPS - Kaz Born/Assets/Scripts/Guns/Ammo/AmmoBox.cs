using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public GunType gunType;
    public float respawnTime = 5f;

    private BoxCollider _boxCollider;
    private List<Transform> children = new List<Transform>();

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
    }

    private void Update()
    {
        transform.Rotate(0, 10 * Time.deltaTime, 0);
    }

    public void Deactivate()
    {
        _boxCollider.enabled = false;
        foreach (Transform child in children)
        {
            child.gameObject.SetActive(false);
        }
        StartCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        yield return new WaitForSeconds(respawnTime);
        _boxCollider.enabled = true;
        foreach (Transform child in children)
        {
            child.gameObject.SetActive(true);
        }
    }
}
