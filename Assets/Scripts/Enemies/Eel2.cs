using System;
using Core;
using PolyNav;
using UnityEngine;

namespace enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Eel2 : MonoBehaviour, IEel
    {
        public Transform[] waypoints;
        public float normalMoveSpeed = 5;

        [Header("ChaseSettings")] [SerializeField]
        private float chaseMoveSpeed = 9;

        [SerializeField] private float chaseMultiplier = 4;


        private Waypoints _waypoints;
        private EelMover _navAgent;
        private IChaseTarget _chaseTarget;
        private Transform _currentWaypoint;


        private IWaypointProvider CurrentWaypoint => _waypoints;


        public Transform CurrentTarget
        {
            get => _chaseTarget == null ? _currentWaypoint : _chaseTarget.transform;
        }

        private void Awake()
        {
            _navAgent = GetComponent<EelMover>();
            _waypoints = new Waypoints(this.waypoints, transform);
            _navAgent.CurrentMoveSpeed = normalMoveSpeed;
        }


        private void Update()
        {
            if (CurrentTarget == null) _currentWaypoint = _waypoints.GetNextWaypoint();
            
            var targetPosition = CurrentTarget.position;
            if (_chaseTarget != null)
            {
                targetPosition += (Vector3) (_chaseTarget.rigidbody2D.velocity * (Time.deltaTime * chaseMultiplier));
            }

            _navAgent.SetDestination(targetPosition);

            if (_navAgent.WasDestinationReached())
            {
                _currentWaypoint = null;
            }
        }

        public void StopChasing()
        {
            _chaseTarget = null;
            _currentWaypoint = null;
            _navAgent.CurrentMoveSpeed = normalMoveSpeed;
        }

        public void StartChasing(Transform transform)
        {
            this._chaseTarget = transform.GetComponent<IChaseTarget>();
            this._currentWaypoint = transform;
            _navAgent.CurrentMoveSpeed = chaseMoveSpeed;
        }
    }
}