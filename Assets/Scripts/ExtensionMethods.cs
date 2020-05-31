using UnityEngine;

public static class ExtensionMethods
{
    public static Vector2Int RoundToInt(this Vector3 vec3)
    {
        return new Vector2Int(Mathf.RoundToInt(vec3.x), Mathf.RoundToInt(vec3.y));
    }
    public static Vector2Int RoundToInt(this Vector2 vec2)
    {
        return new Vector2Int(Mathf.RoundToInt(vec2.x), Mathf.RoundToInt(vec2.y));
    }
}
