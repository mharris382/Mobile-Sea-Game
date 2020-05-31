using UnityEngine;
using UnityEngine.InputSystem;

public class SnapToDirectionProcessor : InputProcessor<Vector2>
{
    [Range(0,1)]
    public float snapThreshold = 0.5f;
    public override Vector2 Process(Vector2 value, InputControl control)
    {
        var absx = Mathf.Abs(value.x);
        var absy = Mathf.Abs(value.y);
        var x = (absx < 0.5f) ? 0 : Mathf.Sign(value.x);
        var y = (absy < 0.5f) ? 0 : Mathf.Sign(value.y);
        return new Vector2(x, y);
    }
}