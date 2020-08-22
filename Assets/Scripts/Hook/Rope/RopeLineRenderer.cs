﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class RopeLineRenderer : MonoBehaviour
{
    [Required]
    public LineRenderer lineRenderer;

    [Required]
    public RopeTest rope;

    private void Start()
    {
    }

    private void Update()
    {
        UpdateRenderer();
    }

    private void UpdateRenderer()
    {
        List<Vector3> points = new List<Vector3>(lineRenderer.positionCount);
        int cnt = 0;
        Vector2 direction = Vector2.down;
        foreach (var pos in rope.JointPositions)
        {
            points.Add(pos);    
            cnt++;
        }

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = cnt;
        lineRenderer.SetPositions(points.ToArray());
    }
}