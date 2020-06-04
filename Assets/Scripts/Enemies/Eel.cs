using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Core;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Eel : MonoBehaviour
    {
        public float maxMoveSpeed = 2;
        public float smoothing = 0.1f;
        public float rotateSmoothing = 0.1f;
        public Transform[] waypoints;
        public float disruptDistance = 1;


        private Transform _currentTarget;
        
        //TODO: implement a helper class for using strategy pattern to change the behaviour 
        private IWaypointProvider _waypoints;
        private IWaypointProvider _disruptedWaypoint;
        private IWaypointProvider CurrentWaypoint => _disruptedWaypoint ?? _waypoints;

        private Vector3 _smoothDampVelocity; //variable used for unity's smooth damp method
        private Vector3 _dampRotation;
        private CheckForDiverHit _checkHitDiver;


        //TODO: Refactor disruption code into a seprate class
        private Transform _disruptedTarget;
        private bool _disrupted = false;

        [System.Obsolete("Now uses _waypoints")]
        private int _index = 0;


        private void Awake()
        {
            this._waypoints = new Waypoints(this.waypoints, transform);
            this._checkHitDiver = new CheckForDiverHit(transform, 0.125f);

            GameManager.OnPlayerHiddenChanged += isHidden =>
            {
                if (isHidden && _currentTarget != null && _currentTarget.CompareTag("Player"))
                {
                    _currentTarget = null;
                }
            };
            
            var disruptedWaypoint = new DisruptedMovement(transform, ref disruptDistance);
            GameManager.OnDisruptorChanged += isDisrupted =>
            {
                _disruptedWaypoint = isDisrupted ? disruptedWaypoint : null;
            };
        }


        private void Update()
        {
            //should only be null when stopped chasing a target and waypoint hasn't been set yet
            if (_currentTarget == null)
                _currentTarget = GetTarget();


            RotateTowardsTarget();
            MoveTowardsTarget();
            if (WasDiverEaten()) return;
            if (WasTargetReached())
            {
                //TODO: Refactor disruption code
                if (_disrupted)
                    MoveDisruptedTarget();

                //Reset the target so that a new target will be found
                _currentTarget = null;
            }
        }


        //Can override to implement the black eels (immune to disruptions)

        protected virtual Transform GetTarget()
        {
            return _disrupted ? _disruptedTarget : this._waypoints.GetCurrentWaypoint();
        }


        #region [Disruption Code]

        //TODO: Refactor disruption code into a separate class


        private void MoveDisruptedTarget()
        {
            if (_disrupted == false)
            {
                _disruptedTarget.position = _waypoints.GetCurrentWaypoint().position;
                return;
            }

            int x = RandomInt();
            int y = RandomInt();
            var direction = new Vector2(x, y);

            Vector2 pos = _disruptedTarget.transform.position;
            pos += (direction * disruptDistance);
            _disruptedTarget.position = pos;
            _currentTarget = _disruptedTarget;
        }

        private static int RandomInt()
        {
            return Mathf.RoundToInt(UnityEngine.Random.Range(-1, 2));
        }

        #endregion


        private bool IsTargetingDiver()
        {
            return _currentTarget != null && _currentTarget.CompareTag("Player");
        }

        private bool WasDiverEaten()
        {
            if (!IsTargetingDiver()) return false;
            var hitDiver = _checkHitDiver.IsDiverInRadius();
            if (hitDiver)
            {
                var deathMessage = "Diver was eaten by an eel!";
                GameManager.Instance.EatDiver(deathMessage);
            }
            return hitDiver;
        }

        private void MoveTowardsTarget()
        {
            var targetPos = _currentTarget.position;
            var curPos = transform.position;
            transform.position = Vector3.SmoothDamp(curPos, targetPos, ref _smoothDampVelocity, smoothing, maxMoveSpeed,
                Time.deltaTime);
        }


        private void RotateTowardsTarget()
        {
            var curr = transform.forward;
            var targetDirection = (_currentTarget.position - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSmoothing * Time.deltaTime);
            //transform.forward = Vector3.SmoothDamp(curr, targetDirection, ref _dampRotation, rotateSmoothing);
        }


        private bool WasTargetReached()
        {
            var dist = (_currentTarget.position - transform.position).sqrMagnitude;
            return (dist < (0.125f * 0.125f));
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !GameManager.Instance.IsPlayerHidden)
            {
                _currentTarget = other.transform;
            }
            else if (other.CompareTag("Player") && _currentTarget == other.transform &&
                     GameManager.Instance.IsPlayerHidden)
            {
                _currentTarget = null;
            }
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && (_currentTarget == other.transform))
            {
                _currentTarget = null;
            }
        }


        #region Obsolete Functions

        [Obsolete("Moved functionality into waypoints")]
        public void NextWp()
        {
            _index += 1;
            _index %= waypoints.Length;
        }

        [Obsolete("Moved functionality into waypoints")]
        private Transform GetCurrentWaypoint()
        {
            var wp = waypoints[_index];
            _currentTarget = wp;
            if (WasTargetReached())
            {
                NextWp();
                wp = waypoints[_index];
            }

            Debug.Assert(wp != null, "Waypoint is null", this);
            return wp;
        }

        #endregion
    }
}