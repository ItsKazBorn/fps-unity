using System.Collections;
using UnityEngine;

public class Sniper : Gun
{
    public GameObject scopeOverlay;
    public GameObject weaponCamera;
    public float scopedFOV = 15f;
    private float normalFOV = 60f;

    protected override void Scope()
    {
        base.Scope();

        if (isScoped)
            StartCoroutine(OnScoped());
        else
            OnUnscoped();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnUnscoped();
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(0.15f);
        scopeOverlay.SetActive(true);
        weaponCamera.SetActive(false);

        fpsCam.fieldOfView = scopedFOV;
    }

    void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        weaponCamera.SetActive(true);

        fpsCam.fieldOfView = normalFOV;
    }
}
