using System;
using System.Collections;
using Core.State;
using UnityEngine;

namespace Player.Diver
{
    public class DiverMovement : IState
    {
        private Rigidbody2D _rigidbody2D;
        private DiverActions _diverActions;
        private Vector2 _slowingVelocity;
        private Vector2 _moveDirection;
        private bool _isHoldingFastMoveButton;

        public DiverMovement(Rigidbody2D rigidbody2D, DiverActions diverActions)
        {
            _rigidbody2D = rigidbody2D;
            _diverActions = diverActions;
            _isHoldingFastMoveButton = false;


            _isHoldingFastMoveButton = false;
            _diverActions.ToggleFastMove.started += context => _isHoldingFastMoveButton = true;
            _diverActions.ToggleFastMove.performed += context => _isHoldingFastMoveButton = false;


            _diverActions.MoveAction.performed += context => _moveDirection = context.ReadValue<Vector2>();
        }

        public IEnumerator OnStateEnter()
        {
            yield break;
        }

        public IEnumerator OnStateExit()
        {
            yield break;
        }


        public void Tick()
        {
        }

        public void FixedTick()
        {
            if (_moveDirection.sqrMagnitude < Mathf.Epsilon)
            {
                var vel = Vector2.SmoothDamp(_rigidbody2D.velocity, Vector2.zero, ref _slowingVelocity, 0.05f);
                _rigidbody2D.velocity = vel;
                return;
            }

            var speed = DiverConfig.GetDiverMoveSpeed(_isHoldingFastMoveButton);
            _rigidbody2D.velocity = _moveDirection.normalized * speed;
        }
    }
}