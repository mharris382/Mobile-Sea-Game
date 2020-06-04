using UnityEngine;


namespace enemies
{
    public class CheckForDiverHit
    {
        private int _diverLayer = 1 << 8;
        private Transform transform;
        
        public float radius = 0.125f;

        public CheckForDiverHit(Transform transform, float radius)
        {
            this.transform = transform;
            this.radius = radius;

        }

        public bool IsDiverInRadius()
        {
            var collider = Physics2D.OverlapCircle(transform.position, radius, _diverLayer);
            return collider != null;
        }
    }
}