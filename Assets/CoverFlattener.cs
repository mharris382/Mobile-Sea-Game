using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverFlattener : MonoBehaviour
{
    [SerializeField] private float flatAngle = 80;
    
    private bool isFlat = false;
    
    
    public void Flatten()
    {
        if (isFlat) return;
        isFlat = true;
        transform.Rotate(flatAngle, 0, 0, Space.Self);
        
    }



    public void Straighten()
    {
        if (!isFlat) return;
        isFlat = false;
        transform.Rotate(-flatAngle, 0, 0, Space.Self);
    }
}
