using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    public int ammountOfPellets = 10;

    protected override void CalculateBullet()
    {
        for (int i = 0; i < ammountOfPellets; i++)
        {
            Vector3 shootDirection = CalculateSpread();
            HitCalculation(shootDirection);
        }
    }

    protected override Vector3 CalculateSpread()
    {
        // Get Shoot Direction
        Vector3 shootDirection = fpsCam.transform.forward;
        // Get Random Direction
        Vector3 spread = new Vector3(UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)).normalized;
        // Multiply by intensity of spread
        float multiplier = UnityEngine.Random.Range(0f, maxSpread);
        spread *= multiplier;
        // Change shoot direction by spread
        shootDirection = shootDirection + fpsCam.transform.TransformDirection(
            new Vector3(spread.x,spread.y));
        return shootDirection;
    }

    protected override void DoDamage(RaycastHit hit)
    {
        Shootable target = hit.transform.GetComponent<Shootable>();
        if (target != null)
        {
            float finalDamage = damage / hit.distance * 10f;
            target.TakeDamage(finalDamage);
        }
    }
}
