﻿using System;
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
    public class DiverSmoothMovement : MonoBehaviour, IChaseTarget, IState, IListenForMoveInput, IDiverMovement
    {
        [Min(0)] public float moveSpeed = 2;
        public DiverConfig config;
        public LayerMask diverCollisionLayers = 1;
        public float MoveSpeed => config == null ? moveSpeed : config.moveSpeed;

        private Vector2 _moveDirection = Vector2.zero;
        private Rigidbody2D _rigidbody2D;
        private Vector2 _slowingVelocity;

        public Vector2 MoveDirection => _moveDirection;

        public new Rigidbody2D rigidbody2D
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

        private void FixedUpdate()
        {
            var moveDirection = _moveDirection;
            if (_moveDirection.sqrMagnitude < Mathf.Epsilon)
            {
                var vel = _rigidbody2D.velocity;
                vel=  Vector2.SmoothDamp(vel, Vector2.zero, ref _slowingVelocity, 0.05f, moveSpeed);
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
            var inputDirection = context.ReadValue<Vector2>().normalized;
            if (Vector2.Dot(inputDirection, _moveDirection) < -0.25f)
            {
                _rigidbody2D.velocity /= 4;
            }
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

        private Vector2 _currentMoveDirection;
        private Vector2 _lastNonSlowingDirection;

        public void FixedTick()
        {
            
        }

        #endregion
    }
    
    
    
    public interface IDiverMovement
    {
        Rigidbody2D rigidbody2D { get; }
        Vector2 MoveDirection { get; }
    }
}

namespace UnityEngine.InputSystem.OnScreen
{
}