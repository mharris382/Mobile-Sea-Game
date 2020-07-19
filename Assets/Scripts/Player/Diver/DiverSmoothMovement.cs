using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.State;
using enemies;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

namespace Player.Diver
{
    public class DiverSmoothMovement : MonoBehaviour, IChaseTarget, IState, IListenForMoveInput, IDiverMovement
    {
        
        [ShowIf(("IsKinematic"))] public LayerMask diverCollisionLayers = 1;

        
        [Obsolete("DiverConfig is now resources singleton")] 
        private float _moveSpeed = 2;
        private DiverConfig _config;
        private Vector2 _moveDirection = Vector2.zero;
        private Rigidbody2D _rigidBody2D;
        private Vector2 _slowingVelocity;

        
        
        public Vector2 MoveDirection => _moveDirection;
        public float MoveSpeed => _config == null ? _moveSpeed : _config.moveSpeed;

        public new Rigidbody2D rigidbody2D
        {
            get => _rigidBody2D;
            set => _rigidBody2D = value;
        }

        private void Awake()
        {
            _config = DiverConfig.Instance;
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }
        
        private void FixedUpdate()
        {
            if (CheckForStopping()) 
                return;

            HandleMovementPhysics();

            ClampPosition();
        }

        private void HandleMovementPhysics()
        {
            Vector2 moveDirection = _moveDirection;
            HandleKinematicCollisions(ref moveDirection, MoveSpeed);

            _rigidBody2D.velocity = moveDirection * MoveSpeed;
        }

        private bool CheckForStopping()
        {
            if (_moveDirection.sqrMagnitude < Mathf.Epsilon)
            {
                var vel = _rigidBody2D.velocity;
                vel = Vector2.SmoothDamp(vel, Vector2.zero, ref _slowingVelocity, 0.05f, _moveSpeed);
                //if ((hit = CheckForCollision(vel.magnitude)) && Vector2.Dot(hit.normal, vel.normalized) > 0.25f) vel = Vector2.zero;

                _rigidBody2D.velocity = vel;
                return true;
            }

            return false;
        }


        //NOTE: Kinematic code will probably no longer be used because we switched to dynamic rigidbody physics
        private void HandleKinematicCollisions(ref Vector2 moveDirection, float speed)
        {
            RaycastHit2D CheckForCollision(float spd)
            {
                var hit = Physics2D.Raycast(transform.position, _moveDirection, spd * Time.fixedDeltaTime, diverCollisionLayers);
                Debug.DrawRay(transform.position, _moveDirection * (spd * Time.fixedDeltaTime), hit ? Color.red : Color.green);

                return hit;
            }
            
            if (moveDirection != Vector2.zero && rigidbody2D.isKinematic)
            {
                var hit = CheckForCollision(speed);
                if (hit) moveDirection = Vector2.zero;
            }
        }

        


        private void ClampPosition()
        {
            void ClampPositionToLevel(ref Vector3 position)
            {
                var rect = GameManager.Instance.CurrentLevel.GetLevelRect();

                position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
                position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);
            }

            Vector3 currPosition = transform.position;
            ClampPositionToLevel(ref currPosition);
            transform.position = currPosition;
        }


        
        public void OnMove(InputAction.CallbackContext context)
        {
            var inputDirection = context.ReadValue<Vector2>().normalized;
            if (Vector2.Dot(inputDirection, _moveDirection) < -0.25f)
            {
                _rigidBody2D.velocity /= 4;
            }

            _moveDirection = context.ReadValue<Vector2>().normalized;
        }

        
        //this is used for touch input on mobile devices
        public void OnMove_OnScreen(Vector2 movement)
        {
            _moveDirection = movement;
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


        #region [Editor Only]

#if UNITY_EDITOR
        
        //referenced through reflection by Odin
        bool IsKinematic() => (_rigidBody2D ? _rigidBody2D : (_rigidBody2D = GetComponent<Rigidbody2D>())).isKinematic;
        
#endif

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