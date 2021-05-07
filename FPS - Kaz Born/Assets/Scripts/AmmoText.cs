using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoText : MonoBehaviour
{
    private TextMeshProUGUI text;
    
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        GameEvents.current.onAmmoChanged += OnAmmoChanged;
    }

    private void OnDestroy()
    {
        GameEvents.current.onAmmoChanged -= OnAmmoChanged;
    }

    private void OnAmmoChanged(int inMag,  int stored)
    {
        text.text = inMag + " / " + stored;
    }
}
