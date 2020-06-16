using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace enemies
{
    [Serializable]
    public class Waypoints : IWaypointProvider
    {
        public List<Transform> waypoints;
        private Transform transform;
        private Dictionary<int, IWaitPoint> waitpoints;

        public Waypoints(Transform[] waypoints, Transform transform)
        {
            this.waypoints = new List<Transform>(waypoints);
            this.transform = transform;
            this.waitpoints = new Dictionary<int, IWaitPoint>();
            for(int i = 0; i < this.waypoints.Count; i++)
            {
                var waitpoint = waypoints[i].GetComponent<IWaitPoint>();
                if(waitpoint == null)continue;
                waitpoints.Add(i, waitpoint);
            }
        }

        private int _index = 0;
        private bool _isWaiting;
        
        private void NextWp()
        {
            if (_isWaiting)
            {
                return;
            }
            if (waitpoints.ContainsKey(_index) )
            {
                _isWaiting = true;
                GameManager.Instance.StartCoroutine(WaitFor(waitpoints[_index].waitTime));
            }
            else
            {
                _index += 1;
                _index %= waypoints.Count;
            }
        }

        private IEnumerator WaitFor(float time)
        {
            yield return new WaitForSeconds(time);
            _isWaiting = false;
            _index += 1;
            _index %= waypoints.Count;
        }

        public Transform GetCurrentWaypoint()
        {
            var wp = waypoints[_index];
            var dist = (wp.position - transform.position).sqrMagnitude;
            if (!(dist < (0.125f * 0.125f))) return wp;
            
            NextWp();
            wp = waypoints[_index];
            
            return wp;
        }
    }

    public interface IWaypointProvider
    {
        Transform GetCurrentWaypoint();
    }
}