using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

namespace Diver
{
    public class DiverSmoothMovement : MonoBehaviour, IChaseTarget
    {
        [Min(0)] public float moveSpeed = 2;
        public DiverConfig config;
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

        private void FixedUpdate()
        {
            _rigidbody2D.velocity = _moveDirection * MoveSpeed;
            Vector3 currPosition = transform.position;
            ClampPositionToLevel(ref currPosition);
            transform.position = currPosition;
            // currPosition += _moveDirection * moveSpeed * Time.deltaTime;
            // transform.position = currPosition;
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
    }


   
}

namespace UnityEngine.InputSystem.OnScreen
{
}