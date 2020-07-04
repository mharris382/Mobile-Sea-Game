using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class SpriteShapeShadowCaster : MonoBehaviour
{
    private ShadowCaster2D shadowCaster;
    private PolygonCollider2D pc;

    public bool autoGenerate = true;

    // Start is called before the first frame update
    void Start()
    {
        shadowCaster = GetComponent<ShadowCaster2D>();
        pc = GetComponent<PolygonCollider2D>();
        SetShapePathFromCollider();
    }

    private void SetShapePathFromCollider()
    {
        var prop = shadowCaster.GetType().GetField("m_ShapePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lop = new List<Vector3>();
        lop.AddRange(pc.points.Select(t => (Vector3)t));
        prop.SetValue(shadowCaster, lop.ToArray());
    }

    private void OnDrawGizmos()
    {
        if(autoGenerate)
        {
            if(shadowCaster==null) shadowCaster = GetComponent<ShadowCaster2D>();
            if(pc == null) pc = GetComponent<PolygonCollider2D>();
            SetShapePathFromCollider();
        }
    }
    
    
    public void OnMove(InputAction.CallbackContext context)
    {
        context.ReadValue<Vector2>();
        var curRot = transform.rotation;
    }
}
