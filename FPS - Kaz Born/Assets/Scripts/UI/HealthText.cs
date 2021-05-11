using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        GameEvents.current.onQPressed += OnQPressed;
        GameEvents.current.onQReleased += OnQReleased;
    }

    private void OnDestroy()
    {
        GameEvents.current.onQPressed -= OnQPressed;
        GameEvents.current.onQReleased -= OnQReleased;
    }

    private void OnQPressed()
    {
        textMesh.text = "Q is Pressed!";
    }

    public void OnQReleased()
    {
        textMesh.text = "Q was Released!";
    }
    
}
