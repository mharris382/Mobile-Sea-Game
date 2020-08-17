using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class HookLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform surfaceLine;
    public float yOffset =0;
    public float distPerSeg = 1;
    private void Update()
    {
        
        if (lineRenderer != null)
        {
            var position = lineRenderer.transform.position;
            if (distPerSeg <= 0)
            {
                Vector3[] pnts = new Vector3[]
                {
                    new Vector3(position.x, position.y, position.z),
                    new Vector3(position.x, yOffset, position.z)
                };
                lineRenderer.useWorldSpace = true;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(pnts);
            }
            else
            {
                var p1 = new Vector3(position.x, position.y, position.z);
                var p2 = new Vector3(position.x, yOffset, position.z);
                float totalDist = Vector2.Distance(p1, b: p2);
                Vector3[] interpolatedPnts = new Vector3[Mathf.CeilToInt(totalDist/distPerSeg)];
                float t = 0;
                for (int i = 0; i < interpolatedPnts.Length; i++)
                {
                    t = i / (float)interpolatedPnts.Length;
                    interpolatedPnts[i] = Vector3.Lerp(p1, p2, t);
                }

                interpolatedPnts[interpolatedPnts.Length - 1] = p2;
                lineRenderer.useWorldSpace = true;
                lineRenderer.positionCount = interpolatedPnts.Length;
                lineRenderer.SetPositions(interpolatedPnts);
            }
            
            
        }
    }
}
