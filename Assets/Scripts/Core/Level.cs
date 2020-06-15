using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Level : MonoBehaviour
{
    public Vector2 diverSpawnPosition;

    [Tooltip("x min and x max for the level bounds")]
    public Vector2Int xRange = new Vector2Int(-5, 5);
    
    [Tooltip("y min and y max for the level bounds")]
    public Vector2Int yRange = new Vector2Int(-10, 1);

    public BoxCollider cameraBounds;

    private float XMin => xRange.x;
    private float XMax => xRange.y;
    private float YMin => yRange.x;
    private float YMax => yRange.y;

    public Rect GetLevelRect() => Rect.MinMaxRect(XMin, YMin, XMax, YMax);
    
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(diverSpawnPosition, 0.25f);
        UnityEditor.Handles.DrawSolidRectangleWithOutline(GetLevelRect(), Color.clear, Color.red.WithAlpha(0.75f));
        if (cameraBounds != null)
        {
            var levelRect = GetLevelRect();
            cameraBounds.size = new Vector3( levelRect.size.x,  levelRect.size.y , 10000);
            cameraBounds.center = levelRect.center;
            
        }
    }
#endif
}
