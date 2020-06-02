using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Diver
{
    public class DiverMoveController : MonoBehaviour
    {

        [Tooltip("Distance to move in x and y")] public int moveDistance = 1;
        [Tooltip("Amount of time for lerp to complete")] public float moveTime = 0.25f;
        public AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        private bool _isMoving = false;
        private float _minPointerDistanceThreshold = 0;
        private Vector2 _pressedPosition;

        private MoveData _currLerp;
        struct MoveData
        {
            public Vector2 startPos;
            public Vector2 endPos;
            public float timePassed;
            //public float endTime;
            public MoveData(Vector2Int start, Vector2Int end, float timePassed = 0)
            {
                this.startPos = start;
                this.endPos = end;
                this.timePassed = timePassed;
            }


        }

        void Update()
        {

            if (Pointer.current.press.wasPressedThisFrame)
            {
                _pressedPosition = Pointer.current.position.ReadValue();
            }
            if (Pointer.current.press.wasReleasedThisFrame)
            {
                var releasePosition =  Pointer.current.position.ReadValue();
                var direction = CalculateDirection(_pressedPosition, releasePosition);
                StartLerp(direction);
            }

            if (_isMoving)
            {
                LerpPosition();
            }

        }

        private void LerpPosition()
        {
            _currLerp.timePassed += (Time.deltaTime / this.moveTime);
            transform.position = Vector3.LerpUnclamped(_currLerp.startPos, _currLerp.endPos, moveCurve.Evaluate(_currLerp.timePassed));

            if (_currLerp.timePassed >= 1)
                _isMoving = false;
        }

        private void StartLerp(Vector2 direction)
        {
            var curPos = transform.position;

            //curPos.x = Mathf.RoundToInt(curPos.x); 
            //curPos.y = Mathf.RoundToInt(curPos.y);

            var moveX =  Vector3.right * direction.x * moveDistance;
            var moveY =  Vector3.up * direction.y * moveDistance;
            var endPos = curPos + moveX + moveY;

            _currLerp = new MoveData(curPos.RoundToInt(), endPos.RoundToInt());
            _isMoving = true;
        }


        /// <summary>
        /// Calculates a direction vector using 4 directions
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
        private Vector2 CalculateDirection(Vector2 startPos, Vector2 endPos)
        {
            Vector2 delta = endPos - startPos;
            if(delta.sqrMagnitude < Mathf.Pow(this._minPointerDistanceThreshold, 2))
            {
                delta = Vector2.zero;
            }
            delta.Normalize();
            //return delta;
            return new SnapToDirectionProcessor().Process(delta, null);
        }


        public void OnMove(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<Vector2>();
            if (! _isMoving &&  direction != Vector2.zero)
            {
                var dir = CalculateDirection(Vector2.zero, direction);
                StartLerp(dir);
            }
        }




    }
}