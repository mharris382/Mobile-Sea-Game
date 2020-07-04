using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class HookLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;


    private void Update()
    {
        
        if (lineRenderer != null)
        {
            Vector3[] pnts = new Vector3[]
            {
                Vector3.zero,
                transform.InverseTransformVector(new Vector3(0, 0 - transform.position.y,0) )
            };
            lineRenderer.SetPositions(pnts);
        }
    }
}
