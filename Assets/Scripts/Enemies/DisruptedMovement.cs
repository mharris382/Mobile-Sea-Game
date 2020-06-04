using System;
using Core;
using UnityEngine;

namespace enemies
{
    public class DisruptedMovement : IWaypointProvider
    {
        private Transform _disruptedTarget;
        private Transform transform;
        private float disruptDistance = 1;

        public DisruptedMovement(Transform transform)
        {
            this.transform = transform;
            this.disruptDistance = 1;
            InitDisruption();
        }    
        public DisruptedMovement(Transform transform,ref float disruptDistance) : this(transform)
        {
            this.disruptDistance = disruptDistance;
        }
        
        private void InitDisruption()
        {
            this._disruptedTarget = new GameObject("Disrupted Target").transform;
            // _disruptedTarget.hideFlags = HideFlags.HideInHierarchy;

            GameManager.OnDisruptorChanged += isDisrupted =>
            {
                if (isDisrupted)
                {
                    _disruptedTarget.position = transform.position;
                }
            };
        }

        private void MoveDisruptedTarget()
        {
            int x = RandomInt();
            int y = RandomInt();
            var direction = new Vector2(x, y);
            
            Vector2 pos = _disruptedTarget.transform.position;
            pos += (direction * disruptDistance);
            _disruptedTarget.position = pos;
        }

        private static int RandomInt()
        {
            return Mathf.RoundToInt( UnityEngine.Random.Range(-1, 2));
        }

        //TODO: finish refactoring disrupted movement
        public Transform GetCurrentWaypoint()
        {
            throw new NotImplementedException();
        }
    }
}