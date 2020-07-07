using System.Collections;
using Core.State;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player.Diver
{
    public class DiverHeavyMovement : IState, IListenForMoveInput
    { 
        [System.Serializable, InlineProperty(LabelWidth = 0)]
        public class Config
        {
            public float jumpForce = 1000;
            public float moveSpeed = 10;
            public float jumpResetTime = 0.5f;
        }
        
        
        private readonly Rigidbody2D _rigidBody2D;
        private readonly DiverActions _diverActions;
        private readonly Config _config;
        private bool _canJump;
        private float _xVelocity;
        
        
        public DiverHeavyMovement(Rigidbody2D rigidBody2D, DiverActions diverActions)
        {
            this._rigidBody2D = rigidBody2D;
            _diverActions = diverActions;
            _config = DiverConfig.Instance.heavyMovementSettings;
            _canJump = true;
            
        }

        public IEnumerator OnStateEnter()
        {
            _diverActions.MoveAction.performed += OnMove;
            yield break;
        }

        public IEnumerator OnStateExit()
        {
            _diverActions.MoveAction.performed -= OnMove;
            yield break;
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
            var vel = _rigidBody2D.velocity;
            vel.x = _xVelocity;
            _rigidBody2D.velocity = vel;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var moveDir = context.ReadValue<Vector2>();
            var dotUp = Vector2.Dot(moveDir, Vector2.up);
            if (_canJump && dotUp > 0.5f)
            {
                _canJump = false;
                _rigidBody2D.AddForce(Vector2.up * _config.jumpForce, ForceMode2D.Impulse);
                CoroutineHandler.instance.Invoke(() => _canJump = true , _config.jumpResetTime);
            }
            else if (_canJump && dotUp < -0.5f)
            {
                _rigidBody2D.AddForce(Vector2.down * _config.jumpForce, ForceMode2D.Impulse);
                _canJump = false;
                CoroutineHandler.instance.Invoke(() => _canJump = true , _config.jumpResetTime);
            }

            _xVelocity = moveDir.x * _config.moveSpeed;
        }
    }
}