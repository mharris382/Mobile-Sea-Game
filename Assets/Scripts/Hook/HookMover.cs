using System;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hook
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class HookMover : MonoBehaviour
    {
        public float raiseLowerSpeed = 10;
        [Range(0,3),ShowIf("useDifferentSmoothings")] public float raiseSmoothing = 0.1f;
        [Range(0,3),ShowIf("useDifferentSmoothings")] public float lowerSmoothing = 1;

        #region [EditorOnly]

#if UNITY_EDITOR

        [SerializeField] private bool useDifferentSmoothings = true;
        [ShowInInspector, HideIf("useDifferentSmoothings"),PropertyRange(0,3)]
        private float Smoothing
        {
            get => raiseSmoothing;
            set => raiseSmoothing = value;
        }
#endif

        #endregion
        
        private Rigidbody2D _rb;
        private TargetJoint2D _targetJoint2D;

        private float _velSmoothing;
        private float _currSpeed;
        private float _currInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _targetJoint2D = GetComponent<TargetJoint2D>();
            _targetJoint2D.target = transform.position;
            _targetJoint2D.autoConfigureTarget = false;
        }

        private void FixedUpdate()
        {
            if ((_targetJoint2D.target - _rb.position).sqrMagnitude > 4)
            {
                var dir = (_targetJoint2D.target - _rb.position).normalized;
                if (Vector2.Dot(new Vector2(0, _currInput), dir) >= 0)
                {
                    _currSpeed = 0;
                    _velSmoothing = 0;
                    return;
                }
            }

            UpdateTargetPosition();

            ClampPosition();
        }

        private void UpdateTargetPosition()
        {
            var velocity = GetVelocity(_currInput);
            _targetJoint2D.target += (velocity * Time.deltaTime);
        }


        private Vector2 GetVelocity(float input)
        {
            var target = input * raiseLowerSpeed;
            var smoothTime = (input > 0 || !useDifferentSmoothings )? raiseSmoothing : lowerSmoothing;
            _currSpeed = Mathf.SmoothDamp(_currSpeed, target, ref _velSmoothing,smoothTime );
            return Vector2.up * _currSpeed;
        }

        private void ClampPosition()
        {
            void ClampPositionToLevel(ref Vector3 position)
            {
                var rect = GameManager.Instance.CurrentLevel.GetLevelRect();

                position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
                position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax + 2f);
            }

            Vector3 currPosition = _targetJoint2D.target;
            ClampPositionToLevel(ref currPosition);
            _targetJoint2D.target = currPosition;
        }


        public void OnHookMove(InputAction.CallbackContext context)
        {
            _currInput = context.ReadValue<float>();
        }
    }
}
