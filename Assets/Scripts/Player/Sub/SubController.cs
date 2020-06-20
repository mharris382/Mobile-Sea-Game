using System.Collections.Generic;
using PolyNav;
using UnityEngine;
using Utilities;

namespace Player.Sub
{
    public class SubController : MonoBehaviour
    {
        public PolyNav.PolyNav2D _map;
        public SubConfig _config;
        public Transform moveToTarget;

        private float maxHorizontalSpeed => _config.maxHorizontalSpeed;
        private float maxDescentSpeed => _config.maxDescentSpeed;
        private float horizontalSmoothing => _config.horizontalSmoothing;
        private float descentSmoothing => _config.descentSmoothing;

        //public float changeDirectionTime = 0.25f;


        private bool _changingDirections;

        private Vector3 _targetPostion;
        private Vector3 _moveVelocity;


        private Rigidbody2D _rb;
        private SpriteRenderer _sr;

        private List<Vector2> _activePath;
        private Vector2 _primeGoal;
        private int _requests;
        private Pathfinder _pathfinder;

        private void Awake()
        {
            // _pathfinder = new Pathfinder(_map, transform, true);
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
            _targetPostion = transform.position;
        }

        private void Update()
        {
            if (moveToTarget != null && _pathfinder!= null)
            {
                _pathfinder.SetDestination(moveToTarget.position, true);
            }
        }


        private void FixedUpdate()
        {
            if (_pathfinder.hasPath == false) return;

            var targetPos = _changingDirections
                ? (Vector2) (transform.position + _moveVelocity)
                : _pathfinder.GetNextPoint();

            var targetX = _targetPostion.x;
            var curX = transform.position.x;
            var xDir = curX - targetX;
            CheckForDirectionChange(xDir);
            var newX = Mathf.SmoothDamp(curX, targetX, ref _moveVelocity.x, horizontalSmoothing, maxHorizontalSpeed);

            var curY = transform.position.y;
            var targetY = _targetPostion.y;
            var newY = Mathf.SmoothDamp(curY, targetY, ref _moveVelocity.y, descentSmoothing, maxDescentSpeed);

            _rb.position = new Vector2(newX, newY);
        }

        public void SetConfig(SubConfig config)
        {
            this._config = config;
        }

        public void SetMap(PolyNav2D map)
        {
            this._map = map;
            _pathfinder = new Pathfinder(map, transform, true);
        }

        public void SetTarget(Transform moveTarget)
        {
            this.moveToTarget = moveTarget;
        }

        private void CheckForDirectionChange(float xDir)
        {
            bool lastDir = _sr.flipX;
            _sr.flipX = xDir > 0;
            //if (_sr.flipX != lastDir)
            //{
            //    FlipDirections();
            //}
        }

        //private void FlipDirections()
        //{
        //    _changingDirections = true;
        //    Invoke("CompleteFlip", changeDirectionTime);
        //}

        //private void CompleteFlip()
        //{
        //    _changingDirections = false;
        //}
    }
}