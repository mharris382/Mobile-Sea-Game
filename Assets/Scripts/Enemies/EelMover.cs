using System.Collections.Generic;
using System.Linq;
using PolyNav;
using UnityEngine;

namespace enemies
{
    public class EelMover : MonoBehaviour
    {
        public const float ARRIVAL_DISTANCE = 0.125f;
        
        public PolyNav2D _map;
        public float smoothing = 0.1f;
        public float rotateSmoothing = 0.1f;

        private Vector3 _smoothDampVelocity; //variable used for unity's smooth damp method
        private Vector3 _dampRotation;

        public float CurrentMoveSpeed { get; set; }

        private Vector2 _primeGoal;

        public Vector2 primeGoal
        {
            get => _primeGoal;
            private set => _primeGoal = value;
        }

        private Vector2 currentTarget;
        
        private int requests = 0;
        private List<Vector2> _activePath = new List<Vector2>();
        private float _maxSpeed;

        public List<Vector2> activePath {
            get => _activePath;
            set
            {
                _activePath = value;
                if ( _activePath.Count > 0 && _activePath[0] == position ) {
                    _activePath.RemoveAt(0);
                }
            }
        }
        
        public PolyNav2D map {
            get => _map != null ? _map : PolyNav2D.current;
            set => _map = value;
        }

        public bool pathPending => requests > 0;
        public bool hasPath => activePath.Count > 0;
        public Vector2 nextPoint => hasPath ? activePath[0] : (Vector2)transform.position;
        public Vector2 position => transform.position;

        

        private void Awake()
        {
            _primeGoal = position;
            if ( _map == null ) {
                _map = FindObjectsOfType<PolyNav2D>().FirstOrDefault(m => m.PointIsValid(position));
            }
        }


        public bool SetDestination(Vector2 goal)
        {
            if (map == null)
            {
                Debug.LogError("No PolyNav2D assigned or in scene!");
                return false;
            }
            
            if ( ( goal - primeGoal ).sqrMagnitude < Mathf.Epsilon )
                return true;
            
            
            primeGoal = goal;
            
            if ((goal - position).sqrMagnitude < ARRIVAL_DISTANCE)
            {
                return true;
            }

            if (!map.PointIsValid(goal))
            {
                SetDestination(map.GetCloserEdgePoint(goal));
                return true;
            }
            
            if ( requests > 0 ) {
                return true;
            }

            requests++;
            
            map.FindPath(position, goal, SetPath);
            
            return true;
        }

        void SetPath(Vector2[] path)
        {
            Debug.Log($"SetPath called with path: {path}");
            if ( requests == 0 ) {
                return;
            }

            requests--;
            
            if(path == null || path.Length == 0)
                return;

            activePath = path.ToList();
        }
        
        private bool WasNavPointReached()
        {
            var dist = (nextPoint - position).sqrMagnitude;
            return (dist < (ARRIVAL_DISTANCE * ARRIVAL_DISTANCE));
        }

        public bool WasDestinationReached()
        {
            var dist = (primeGoal - position).sqrMagnitude;
            return (dist < (ARRIVAL_DISTANCE * ARRIVAL_DISTANCE));
        }

        private void Update()
        {
            if (hasPath == false)
            {
                //Debug.Log($"{name} has no path.  (PathPending = {pathPending})");
                return;
            }

            if (WasNavPointReached())
            {
                if(activePath.Count > 1)
                    activePath.RemoveAt(0);
                
            }
            
            MoveTowardsTarget();
            RotateTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            Vector2 targetPos = nextPoint;
            Vector2 curPos = this.position;
            transform.position = Vector3.SmoothDamp(curPos, targetPos, ref _smoothDampVelocity, smoothing, CurrentMoveSpeed,
                Time.deltaTime);
        }
        
        private void RotateTowardsTarget()
        {
            var curr = transform.forward;
            var targetDirection = (nextPoint - position).normalized;
            var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSmoothing * Time.deltaTime);
            //transform.forward = Vector3.SmoothDamp(curr, targetDirection, ref _dampRotation, rotateSmoothing);
        }
    }
}