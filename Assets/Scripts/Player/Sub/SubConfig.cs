using UnityEngine;

namespace Player.Sub
{
    [CreateAssetMenu(menuName = "Data/Sub Config")]
    public class SubConfig : ScriptableObject
    {
        public float maxHorizontalSpeed = 15f;
        public float maxDescentSpeed = 5f;
        [Range(0,1)] public float horizontalSmoothing = 0.1f;
        [Range(0, 1)] public float descentSmoothing = 0.5f;
    }
}