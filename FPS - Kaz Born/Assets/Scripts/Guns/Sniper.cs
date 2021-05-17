using System;
using System.Collections;
using UnityEngine;

public class Sniper : Gun
{
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private GameObject scopeOverlay;
    [SerializeField] private GameObject weaponCamera;
    [SerializeField] private float scopedFOV = 15f;
    private float normalFOV = 60f;

    public override void Scope()
    {
        base.Scope();
        StartCoroutine(OnScoped());
    }

    public override void UnScope()
    {
        base.UnScope();
        OnUnscoped();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnUnscoped();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnUnscoped();
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(0.15f);
        scopeOverlay.SetActive(true);
        weaponCamera.SetActive(false);

        fpsCamera.fieldOfView = scopedFOV;
    }

    void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        weaponCamera.SetActive(true);

        fpsCamera.fieldOfView = normalFOV;
    }
}
