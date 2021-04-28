using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : Shootable
{
    protected override void Die()
    {
        Destroy(gameObject);
    }
}
