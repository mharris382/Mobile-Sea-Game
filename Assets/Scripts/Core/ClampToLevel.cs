using Core;
using UnityEngine;

public class ClampToLevel
{
    


    public Vector2 ClampPositionToLevel(Vector2 position, bool ignoreYMax= false)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentLevel==null)
        {
            return position;
        }

        var levelRect = GameManager.Instance.CurrentLevel.GetLevelRect();
        position.x = Mathf.Clamp(position.x, levelRect.xMin, levelRect.xMax);
        position.y = Mathf.Clamp(position.y, levelRect.yMin, ignoreYMax ? Mathf.Infinity : levelRect.yMax);
        return position;
    }
}