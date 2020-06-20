using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using PolyNav;
using UnityEngine;

namespace Utilities
{
    public class Pathfinder
    {
        private PolyNav2D _map;
        private Transform transform;
        private float _arrivalDistance = Mathf.Epsilon;

        private int requests = 0;
        private List<Vector2> _activePath = new List<Vector2>();

        private Vector2 _primeGoal;

        public Pathfinder(PolyNav2D map, Transform transform, bool repath)
        {
            _map = map;
            this.transform = transform;
            if (repath)
            {
                Pathfinding.RepathAll += () =>
                {
                    if (!map.CheckLOS(position, nextPoint))
                        Repath();
                };
            }
        }

        #region [Properties]

        public Vector2 primeGoal
        {
            get => _primeGoal;
            private set => _primeGoal = value;
        }

        public List<Vector2> activePath
        {
            get => _activePath;
            set
            {
                _activePath = value;
                if (_activePath.Count > 0 && _activePath[0] == position)
                {
                    _activePath.RemoveAt(0);
                }
            }
        }

        public PolyNav2D map
        {
            get => _map != null ? _map : PolyNav2D.current;
            set => _map = value;
        }

        public bool pathPending => requests > 0;
        public bool hasPath => activePath.Count > 0;

        public Vector2 nextPoint => hasPath ? activePath[0] : (Vector2) transform.position;
        public Vector2 position => transform.position;

        public float ArrivalDistance => Mathf.Max(_arrivalDistance, Mathf.Epsilon);

        #endregion

        void Repath()
        {
            if (requests > 0)
            {
                return;
            }

            requests++;
            map.FindPath(position, primeGoal, SetPath);
        }


        public bool SetDestination(Vector2 goal, bool getCloserPointOnInvalid = true)
        {
            if (map == null)
            {
                Debug.LogError("No PolyNav2D assigned or in scene!");
                return false;
            }

            //if goal is same as current goal do not recalculate for performance
            if ((goal - primeGoal).sqrMagnitude < Mathf.Epsilon)
                return true;

            primeGoal = goal;

            //check if already at goal position
            if ((goal - position).sqrMagnitude < _arrivalDistance)
                return true;


            if (!map.PointIsValid(goal))
            {
                if (getCloserPointOnInvalid)
                {
                    SetDestination(map.GetCloserEdgePoint(goal));
                    return true;
                }
                else
                {
                    return false;
                }
            }


            if (requests > 0)
                return true;
            requests++;
            map.FindPath(position, goal, SetPath);
            return true;
        }

        private bool NavPointReached()
        {
            var dist = (nextPoint - position).sqrMagnitude;
            return (dist < (ArrivalDistance * ArrivalDistance));
        }

        void SetPath(Vector2[] path)
        {
            if (requests == 0)
            {
                return;
            }

            requests--;

            if (path == null || path.Length == 0)
                return;

            activePath = path.ToList();
        }


        // private void OnNavPointReached()
        // {
        //     if (_activePath.Count > 1)
        //         _activePath.RemoveAt(0);
        //     else
        //     {
        //         //TODO: trigger a reached destination event
        //         _activePath = null;
        //     }
        // }

        public Vector2 GetNextPoint()
        {
            if (!hasPath)
                return position;

            if (NavPointReached())
            {
                if (activePath.Count > 1)
                    activePath.RemoveAt(0);
                // OnNavPointReached();
            }

            return nextPoint;
        }

        public Vector2 GetDirectionAlongPath()
        {
            if (hasPath == false)
                return Vector2.zero;

            var nextPoint = GetNextPoint();
            return nextPoint - position;
        }
    }


    public static class Pathfinding
    {
        public static event Action RepathAll;

        static Pathfinding()
        {
            GameManager.Instance.StartCoroutine(PathfindingUpdates());
        }

        static IEnumerator PathfindingUpdates()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                RepathAll?.Invoke();
            }
        }
    }
}