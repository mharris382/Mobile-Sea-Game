using System.Collections;
using Core.State;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player
{
    public class DiverHeavyMovement : IState, IListenForMoveInput
    { 
        [System.Serializable]
        public class Config
        {
            public float jumpForce = 1000;
            public float moveSpeed = 10;
            public float jumpResetTime = 0.5f;
            public bool testing = true;
        }
        private readonly Rigidbody2D _rb;
        private readonly Holder _objHolder;
        private readonly Config _config;
        private bool _canJump;
        private float _xVelocity;
        public DiverHeavyMovement(Rigidbody2D rb, Holder objHolder, Config config)
        {
            this._rb = rb;
            _objHolder = objHolder;
            _config = config;
            _canJump = true;
        }

        public IEnumerator OnStateEnter()
        {
            _rb.isKinematic = false;
            if (_config.testing) _rb.mass = 2.5f;
            yield break;
        }

        public IEnumerator OnStateExit()
        {
            if (_config.testing) _rb.mass = 2f;
            yield break;
        }

        public void Tick()
        {
        }

        public void FixedTick()
        {
            var vel = _rb.velocity;
            vel.x = _xVelocity;
            _rb.velocity = vel;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var moveDir = context.ReadValue<Vector2>();
            var dotUp = Vector2.Dot(moveDir, Vector2.up);
            if (_canJump && dotUp > 0.5f)
            {
                _canJump = false;
                _rb.AddForce(Vector2.up * _config.jumpForce, ForceMode2D.Impulse);
                CoroutineHandler.instance.Invoke(() => _canJump = true , _config.jumpResetTime);
            }
            else if (_canJump && dotUp < -0.5f)
            {
                _rb.AddForce(Vector2.down * _config.jumpForce, ForceMode2D.Impulse);
                _canJump = false;
                CoroutineHandler.instance.Invoke(() => _canJump = true , _config.jumpResetTime);
            }

            _xVelocity = moveDir.x;
        }
    }
}