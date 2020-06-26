using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.State;
using enemies;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

namespace Player.Diver
{
    public class DiverSmoothMovement : MonoBehaviour, IChaseTarget, IState, IListenForMoveInput
    {
        [Min(0)] public float moveSpeed = 2;
        public DiverConfig config;
        public LayerMask diverCollisionLayers = 1;
        public float MoveSpeed => config == null ? moveSpeed : config.moveSpeed;

        private Vector2 _moveDirection = Vector2.zero;
        private Rigidbody2D _rigidbody2D;

        public Rigidbody2D rigidbody2D
        {
            get => _rigidbody2D;
            set => _rigidbody2D = value;
        }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
        }

        private Vector2 slowingVelocity;
        private void FixedUpdate()
        {
            var moveDirection = _moveDirection;
            if (_moveDirection.sqrMagnitude < Mathf.Epsilon)
            {
                var vel = _rigidbody2D.velocity;
                vel=  Vector2.SmoothDamp(vel, Vector2.zero, ref slowingVelocity, 0.05f, moveSpeed);
                RaycastHit2D hit;
                //if ((hit = CheckForCollision(vel.magnitude)) && Vector2.Dot(hit.normal, vel.normalized) > 0.25f) vel = Vector2.zero;
                
                _rigidbody2D.velocity = vel;
                return;
            }
            var speed = MoveSpeed;
            if (moveDirection != Vector2.zero && rigidbody2D.isKinematic)
            {
                var hit = CheckForCollision(speed);
                if(hit)moveDirection = Vector2.zero;
            }

            _rigidbody2D.velocity = moveDirection * speed;
            Vector3 currPosition = transform.position;
            ClampPositionToLevel(ref currPosition);
            transform.position = currPosition;
            // currPosition += _moveDirection * moveSpeed * Time.deltaTime;
            // transform.position = currPosition;
        }

        private RaycastHit2D CheckForCollision(float speed)
        {
            Vector2 moveDirection;
            var hit = Physics2D.Raycast(transform.position, _moveDirection, speed * Time.fixedDeltaTime, diverCollisionLayers);
            Debug.DrawRay(transform.position, _moveDirection * (speed * Time.fixedDeltaTime), hit ? Color.red : Color.green);

            return hit;
        }

        private void ClampPositionToLevel(ref Vector3 position)
        {
            var rect = GameManager.Instance.CurrentLevel.GetLevelRect();

            position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
            position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);
        }

        // Start is called before the first frame update
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>().normalized;
        }

        public void OnMove_OnScreen(Vector2 movement)
        {
            _moveDirection = movement;
        }

        public void SetConfig(DiverConfig config)
        {
            this.config = config;
        }

        #region [IState]
        
        public IEnumerator OnStateEnter()
        {
            enabled = true;
            yield break;
        }

        public IEnumerator OnStateExit()
        {
            enabled = false;
            yield break;
        }

        public void Tick()
        {
            
        }

        public void FixedTick()
        {
            
        }

        #endregion
    }
}

namespace UnityEngine.InputSystem.OnScreen
{
}