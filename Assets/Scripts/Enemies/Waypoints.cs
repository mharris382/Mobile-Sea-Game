using System;
using System.Collections.Generic;
using UnityEngine;

namespace enemies
{
    [Serializable]
    public class Waypoints
    {
        public List<Transform> waypoints;
        private Transform transform;

        public Waypoints(Transform[] waypoints, Transform transform)
        {
            this.waypoints = new List<Transform>(waypoints);
            this.transform = transform;

        }

        private int _index = 0;

        private void NextWp()
        {
            _index += 1;
            _index %= waypoints.Count;
        }

        public Transform GetCurrentWaypoint()
        {
            var wp = waypoints[_index];
            var dist = (wp.position - transform.position).sqrMagnitude;
            if ((dist < (0.125f * 0.125f)))
            {
                NextWp();
                wp = waypoints[_index];
            }

            return wp;
        }
    }
}